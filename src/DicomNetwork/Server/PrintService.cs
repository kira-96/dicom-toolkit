// https://github.com/fo-dicom/fo-dicom-samples/blob/master/Desktop/Print%20SCP/PrintService.cs

// Copyright (c) 2012-2020 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

#if FellowOakDicom5
using FellowOakDicom;
using FellowOakDicom.Log;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Network;
using FellowOakDicom.Printing;
#else
using Dicom;
using Dicom.Log;
using Dicom.Network;
using Dicom.Printing;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Server
{
    internal class PrintService : DicomService, IDicomServiceProvider, IDicomCEchoProvider, IDicomNServiceProvider
    {
        #region Properties and Attributes

        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
        {
           DicomTransferSyntax.ExplicitVRLittleEndian,
           DicomTransferSyntax.ExplicitVRBigEndian,
           DicomTransferSyntax.ImplicitVRLittleEndian
        };

        public string CallingAE { get; protected set; }
        public string CalledAE { get; protected set; }
        public System.Net.IPAddress RemoteIP { get; private set; }

        public Printer Printer => PrintServer.Default.Printer; 

        private FilmSession _filmSession;

        private readonly Dictionary<string, PrintJob> _printJobList = new Dictionary<string, PrintJob>();

        private bool _sendEventReports = false;

        private readonly object _synchRoot = new object();

        #endregion

        #region Constructors and Initialization

#if FellowOakDicom5
        public PrintService(INetworkStream stream, Encoding fallbackEncoding, ILogger logger, ILogManager logManager, INetworkManager networkManager, ITranscoderManager transcoderManager)
            : base(stream, fallbackEncoding, logger, logManager, networkManager, transcoderManager)
#else
        public PrintService(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
#endif
        {
            var pi = stream.GetType().GetProperty("Socket", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pi != null)
            {
                var endPoint = ((System.Net.Sockets.Socket)pi.GetValue(stream, null)).RemoteEndPoint as System.Net.IPEndPoint;
                RemoteIP = endPoint.Address;
            }
            else
            {
                RemoteIP = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
            }
        }

        #endregion

        #region IDicomServiceProvider Members

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            Logger.Info("Received association request from AE: {0} with IP: {1} ", association.CallingAE, RemoteIP);

            if (Printer.PrinterAet != association.CalledAE)
            {
                Logger.Error(
                    "Association with {0} rejected since requested printer {1} not found",
                    association.CallingAE,
                    association.CalledAE);
                return SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    DicomRejectReason.CalledAENotRecognized);
            }

            CallingAE = association.CallingAE;
            CalledAE = association.CalledAE;

            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification
                    || pc.AbstractSyntax == DicomUID.BasicGrayscalePrintManagementMetaSOPClass
                    || pc.AbstractSyntax == DicomUID.BasicColorPrintManagementMetaSOPClass
                    || pc.AbstractSyntax == DicomUID.PrinterSOPClass
                    || pc.AbstractSyntax == DicomUID.BasicFilmSessionSOPClass
                    || pc.AbstractSyntax == DicomUID.BasicFilmBoxSOPClass
                    || pc.AbstractSyntax == DicomUID.BasicGrayscaleImageBoxSOPClass
                    || pc.AbstractSyntax == DicomUID.BasicColorImageBoxSOPClass)
                {
                    pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                }
                else if (pc.AbstractSyntax == DicomUID.PrintJobSOPClass)
                {
                    pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                    _sendEventReports = true;
                }
                else
                {
                    Logger.Warn(
                        "Requested abstract syntax {abstractSyntax} from {callingAE} not supported",
                        pc.AbstractSyntax,
                        association.CallingAE);
                    pc.SetResult(DicomPresentationContextResult.RejectAbstractSyntaxNotSupported);
                }
            }

            Logger.Info("Accepted association request from {callingAE}", association.CallingAE);
            return SendAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            Clean();
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            //stop printing operation
            //log the abort reason
            Logger.Error("Received abort from {0}, reason is {1}", source, reason);
        }

        public void OnConnectionClosed(Exception exception)
        {
            Clean();
        }

        #endregion

        #region IDicomCEchoProvider Members

#if FellowOakDicom5
        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            Logger.Info("Received verification request from AE {0} with IP: {1}", CallingAE, RemoteIP);
            return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
        }
#else
        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            Logger.Info("Received verification request from AE {0} with IP: {1}", CallingAE, RemoteIP);
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }
#endif

        #endregion

        #region N-CREATE requests handlers

#if FellowOakDicom5
        public Task<DicomNCreateResponse> OnNCreateRequestAsync(DicomNCreateRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return Task.FromResult(CreateFilmSession(request));
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return Task.FromResult(CreateFilmBox(request));
                }
                else
                {
                    return Task.FromResult(new DicomNCreateResponse(request, DicomStatus.SOPClassNotSupported));
                }
            }
        }
#else
        public DicomNCreateResponse OnNCreateRequest(DicomNCreateRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return CreateFilmSession(request);
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return CreateFilmBox(request);
                }
                else
                {
                    return new DicomNCreateResponse(request, DicomStatus.SOPClassNotSupported);
                }
            }
        }
#endif

        private DicomNCreateResponse CreateFilmSession(DicomNCreateRequest request)
        {
            if (_filmSession != null)
            {
                Logger.Error("Attemted to create new basic film session on association with {0}", CallingAE);
                SendAbortAsync(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified).Wait();
                return new DicomNCreateResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            var pc = request.PresentationContext;

            bool isColor = pc != null && pc.AbstractSyntax == DicomUID.BasicColorPrintManagementMetaSOPClass;

            _filmSession = new FilmSession(request.SOPClassUID, request.SOPInstanceUID, request.Dataset, isColor);

            Logger.Info("Create new film session {0}", _filmSession.SOPInstanceUID.UID);
            if (request.SOPInstanceUID == null || request.SOPInstanceUID.UID == string.Empty)
            {
                request.Command.AddOrUpdate(DicomTag.AffectedSOPInstanceUID, _filmSession.SOPInstanceUID);
            }
            var response = new DicomNCreateResponse(request, DicomStatus.Success);
            return response;
        }

        private DicomNCreateResponse CreateFilmBox(DicomNCreateRequest request)
        {
            if (_filmSession == null)
            {
                Logger.Error("A basic film session does not exist for this association {0}", CallingAE);
                SendAbortAsync(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified).Wait();
                return new DicomNCreateResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            var filmBox = _filmSession.CreateFilmBox(request.SOPInstanceUID, request.Dataset);

            if (!filmBox.Initialize())
            {
                Logger.Error("Failed to initialize requested film box {0}", filmBox.SOPInstanceUID.UID);
                SendAbortAsync(DicomAbortSource.ServiceProvider, DicomAbortReason.NotSpecified).Wait();
                return new DicomNCreateResponse(request, DicomStatus.ProcessingFailure);
            }

            Logger.Info("Created new film box {0}", filmBox.SOPInstanceUID.UID);
            if (request.SOPInstanceUID == null || request.SOPInstanceUID.UID == string.Empty)
            {
                request.Command.AddOrUpdate(DicomTag.AffectedSOPInstanceUID, filmBox.SOPInstanceUID.UID);
            }

            return new DicomNCreateResponse(request, DicomStatus.Success)
            {
                Dataset = filmBox
            };
        }


        #endregion

        #region N-DELETE request handler

#if FellowOakDicom5
        public Task<DicomNDeleteResponse> OnNDeleteRequestAsync(DicomNDeleteRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return Task.FromResult(DeleteFilmSession(request));
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return Task.FromResult(DeleteFilmBox(request));
                }
                else
                {
                    return Task.FromResult(new DicomNDeleteResponse(request, DicomStatus.NoSuchSOPClass));
                }
            }
        }
#else
        public DicomNDeleteResponse OnNDeleteRequest(DicomNDeleteRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return DeleteFilmSession(request);
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return DeleteFilmBox(request);
                }
                else
                {
                    return new DicomNDeleteResponse(request, DicomStatus.NoSuchSOPClass);
                }
            }
        }
#endif

        private DicomNDeleteResponse DeleteFilmBox(DicomNDeleteRequest request)
        {
            if (_filmSession == null)
            {
                Logger.Error("Can't delete a basic film session doesnot exist for this association {0}", CallingAE);
                return new DicomNDeleteResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            DicomStatus status =
                _filmSession.DeleteFilmBox(request.SOPInstanceUID)
                ? DicomStatus.Success
                : DicomStatus.NoSuchObjectInstance;
            var response = new DicomNDeleteResponse(request, status);
            return response;
        }

        private DicomNDeleteResponse DeleteFilmSession(DicomNDeleteRequest request)
        {
            if (_filmSession == null)
            {
                Logger.Error("Can't delete a basic film session doesnot exist for this association {0}", CallingAE);
                return new DicomNDeleteResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            if (!request.SOPInstanceUID.Equals(_filmSession.SOPInstanceUID))
            {
                Logger.Error(
                    "Can't delete a basic film session with instace UID {0} doesnot exist for this association {1}",
                    request.SOPInstanceUID.UID,
                    CallingAE);
                return new DicomNDeleteResponse(request, DicomStatus.NoSuchObjectInstance);
            }
            _filmSession = null;

            return new DicomNDeleteResponse(request, DicomStatus.Success);
        }

        #endregion

        #region N-SET request handler

#if FellowOakDicom5
        public Task<DicomNSetResponse> OnNSetRequestAsync(DicomNSetRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return Task.FromResult(SetFilmSession(request));
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return Task.FromResult(SetFilmBox(request));
                }
                else if (request.SOPClassUID == DicomUID.BasicColorImageBoxSOPClass
                         || request.SOPClassUID == DicomUID.BasicGrayscaleImageBoxSOPClass)
                {
                    return Task.FromResult(SetImageBox(request));
                }
                else
                {
                    return Task.FromResult(new DicomNSetResponse(request, DicomStatus.SOPClassNotSupported));
                }
            }
        }
#else
        public DicomNSetResponse OnNSetRequest(DicomNSetRequest request)
        {
            lock (_synchRoot)
            {
                if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass)
                {
                    return SetFilmSession(request);
                }
                else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass)
                {
                    return SetFilmBox(request);
                }
                else if (request.SOPClassUID == DicomUID.BasicColorImageBoxSOPClass
                         || request.SOPClassUID == DicomUID.BasicGrayscaleImageBoxSOPClass)
                {
                    return SetImageBox(request);
                }
                else
                {
                    return new DicomNSetResponse(request, DicomStatus.SOPClassNotSupported);
                }
            }
        }
#endif

        private DicomNSetResponse SetImageBox(DicomNSetRequest request)
        {
            if (_filmSession == null)
            {
                Logger.Error("A basic film session does not exist for this association {0}", CallingAE);
                return new DicomNSetResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            Logger.Info("Set image box {0}", request.SOPInstanceUID.UID);

            var imageBox = _filmSession.FindImageBox(request.SOPInstanceUID);
            if (imageBox == null)
            {
                Logger.Error(
                    "Received N-SET request for invalid image box instance {0} for this association {1}",
                    request.SOPInstanceUID.UID,
                    CallingAE);
                return new DicomNSetResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            request.Dataset.CopyTo(imageBox);

            return new DicomNSetResponse(request, DicomStatus.Success);
        }

        private DicomNSetResponse SetFilmBox(DicomNSetRequest request)
        {
            if (_filmSession == null)
            {
                Logger.Error("A basic film session does not exist for this association {0}", CallingAE);
                return new DicomNSetResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            Logger.Info("Set film box {0}", request.SOPInstanceUID.UID);
            var filmBox = _filmSession.FindFilmBox(request.SOPInstanceUID);

            if (filmBox == null)
            {
                Logger.Error(
                    "Received N-SET request for invalid film box {0} from {1}",
                    request.SOPInstanceUID.UID,
                    CallingAE);
                return new DicomNSetResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            request.Dataset.CopyTo(filmBox);

            filmBox.Initialize();

            var response = new DicomNSetResponse(request, DicomStatus.Success);
            response.Command.AddOrUpdate(DicomTag.AffectedSOPInstanceUID, filmBox.SOPInstanceUID);
            response.Dataset = filmBox;
            return response;
        }

        private DicomNSetResponse SetFilmSession(DicomNSetRequest request)
        {
            if (_filmSession == null || _filmSession.SOPInstanceUID.UID != request.SOPInstanceUID.UID)
            {
                Logger.Error("A basic film session does not exist for this association {0}", CallingAE);
                return new DicomNSetResponse(request, DicomStatus.NoSuchObjectInstance);
            }

            Logger.Info("Set film session {0}", request.SOPInstanceUID.UID);
            request.Dataset.CopyTo(_filmSession);

            return new DicomNSetResponse(request, DicomStatus.Success);
        }

        #endregion

        #region N-GET request handler

#if FellowOakDicom5
        public Task<DicomNGetResponse> OnNGetRequestAsync(DicomNGetRequest request)
        {
            lock (_synchRoot)
            {
                Logger.Info(request.ToString(true));

                if (request.SOPClassUID == DicomUID.PrinterSOPClass
                    && request.SOPInstanceUID == DicomUID.PrinterSOPInstance)
                {
                    return Task.FromResult(GetPrinter(request));
                }
                else if (request.SOPClassUID == DicomUID.PrintJobSOPClass)
                {
                    return Task.FromResult(GetPrintJob(request));
                }
                else if (request.SOPClassUID == DicomUID.PrinterConfigurationRetrievalSOPClass
                         && request.SOPInstanceUID == DicomUID.PrinterConfigurationRetrievalSOPInstance)
                {
                    return Task.FromResult(GetPrinterConfiguration(request));
                }
                else
                {
                    return Task.FromResult(new DicomNGetResponse(request, DicomStatus.NoSuchSOPClass));
                }
            }
        }
#else
        public DicomNGetResponse OnNGetRequest(DicomNGetRequest request)
        {
            lock (_synchRoot)
            {
                Logger.Info(request.ToString(true));

                if (request.SOPClassUID == DicomUID.PrinterSOPClass
                    && request.SOPInstanceUID == DicomUID.PrinterSOPInstance)
                {
                    return GetPrinter(request);
                }
                else if (request.SOPClassUID == DicomUID.PrintJobSOPClass)
                {
                    return GetPrintJob(request);
                }
                else if (request.SOPClassUID == DicomUID.PrinterConfigurationRetrievalSOPClass
                         && request.SOPInstanceUID == DicomUID.PrinterConfigurationRetrievalSOPInstance)
                {
                    return GetPrinterConfiguration(request);
                }
                else
                {
                    return new DicomNGetResponse(request, DicomStatus.NoSuchSOPClass);
                }
            }
        }
#endif

        private DicomNGetResponse GetPrinter(DicomNGetRequest request)
        {
            var ds = new DicomDataset();

            var sb = new StringBuilder();
            if (request.Attributes != null && request.Attributes.Length > 0)
            {
                foreach (var item in request.Attributes)
                {
                    sb.AppendFormat("GetPrinter attribute {0} requested", item);
                    sb.AppendLine();
                    var value = Printer.GetSingleValueOrDefault(item, "");
                    ds.Add(item, value);
                }

                Logger.Info(sb.ToString());
            }
            if (!ds.Any())
            {
                ds.Add(DicomTag.PrinterStatus, Printer.PrinterStatus);
                ds.Add(DicomTag.PrinterStatusInfo, "");
                ds.Add(DicomTag.PrinterName, Printer.PrinterName);
                ds.Add(DicomTag.Manufacturer, Printer.Manufacturer);
                ds.Add(DicomTag.DateOfLastCalibration, Printer.DateTimeOfLastCalibration.Date);
                ds.Add(DicomTag.TimeOfLastCalibration, Printer.DateTimeOfLastCalibration);
                ds.Add(DicomTag.ManufacturerModelName, Printer.ManufacturerModelName);
                ds.Add(DicomTag.DeviceSerialNumber, Printer.DeviceSerialNumber);
                ds.Add(DicomTag.SoftwareVersions, Printer.SoftwareVersions);
            }

            var response = new DicomNGetResponse(request, DicomStatus.Success)
            {
                Dataset = ds
            };

            Logger.Info(response.ToString(true));
            return response;
        }

        private DicomNGetResponse GetPrinterConfiguration(DicomNGetRequest request)
        {
            var dataset = new DicomDataset();
            var config = new DicomDataset();

            var sequence = new DicomSequence(DicomTag.PrinterConfigurationSequence, config);

            dataset.Add(sequence);

            var response = new DicomNGetResponse(request, DicomStatus.Success);
            response.Command.AddOrUpdate(DicomTag.AffectedSOPInstanceUID, request.SOPInstanceUID);
            response.Dataset = dataset;
            return response;
        }

        private DicomNGetResponse GetPrintJob(DicomNGetRequest request)
        {
            if (_printJobList.ContainsKey(request.SOPInstanceUID.UID))
            {
                var printJob = _printJobList[request.SOPInstanceUID.UID];

                var sb = new StringBuilder();

                var dataset = new DicomDataset();

                if (request.Attributes != null && request.Attributes.Length > 0)
                {
                    foreach (var item in request.Attributes)
                    {
                        sb.AppendFormat("GetPrintJob attribute {0} requested", item);
                        sb.AppendLine();
                        var value = printJob.GetSingleValueOrDefault(item, "");
                        dataset.Add(item, value);
                    }

                    Logger.Info(sb.ToString());
                }

                var response = new DicomNGetResponse(request, DicomStatus.Success)
                {
                    Dataset = dataset
                };
                return response;
            }
            else
            {
                var response = new DicomNGetResponse(request, DicomStatus.NoSuchObjectInstance);
                return response;
            }
        }

        #endregion

        #region N-ACTION request handler

#if FellowOakDicom5
        public Task<DicomNActionResponse> OnNActionRequestAsync(DicomNActionRequest request)
#else
        public DicomNActionResponse OnNActionRequest(DicomNActionRequest request)
#endif
        {
            if (_filmSession == null)
            {
                Logger.Error("A basic film session does not exist for this association {0}", CallingAE);
#if FellowOakDicom5
                return Task.FromResult(new DicomNActionResponse(request, DicomStatus.InvalidObjectInstance));
#else
                return new DicomNActionResponse(request, DicomStatus.InvalidObjectInstance);
#endif
            }

            lock (_synchRoot)
            {
                try
                {

                    var filmBoxList = new List<FilmBox>();
                    if (request.SOPClassUID == DicomUID.BasicFilmSessionSOPClass && request.ActionTypeID == 0x0001)
                    {
                        Logger.Info("Creating new print job for film session {0}", _filmSession.SOPInstanceUID.UID);
                        filmBoxList.AddRange(_filmSession.BasicFilmBoxes);
                    }
                    else if (request.SOPClassUID == DicomUID.BasicFilmBoxSOPClass && request.ActionTypeID == 0x0001)
                    {
                        Logger.Info("Creating new print job for film box {0}", request.SOPInstanceUID.UID);

                        var filmBox = _filmSession.FindFilmBox(request.SOPInstanceUID);
                        if (filmBox != null)
                        {
                            filmBoxList.Add(filmBox);
                        }
                        else
                        {
                            Logger.Error(
                                "Received N-ACTION request for invalid film box {0} from {1}",
                                request.SOPInstanceUID.UID,
                                CallingAE);
#if FellowOakDicom5
                            return Task.FromResult(new DicomNActionResponse(request, DicomStatus.NoSuchObjectInstance));
#else
                            return new DicomNActionResponse(request, DicomStatus.NoSuchObjectInstance);
#endif
                        }
                    }
                    else
                    {
                        if (request.ActionTypeID != 0x0001)
                        {
                            Logger.Error(
                                "Received N-ACTION request for invalid action type {0} from {1}",
                                request.ActionTypeID,
                                CallingAE);
#if FellowOakDicom5
                            return Task.FromResult(new DicomNActionResponse(request, DicomStatus.NoSuchActionType));
#else
                            return new DicomNActionResponse(request, DicomStatus.NoSuchActionType);
#endif
                        }
                        else
                        {
                            Logger.Error(
                                "Received N-ACTION request for invalid SOP class {0} from {1}",
                                request.SOPClassUID,
                                CallingAE);
#if FellowOakDicom5
                            return Task.FromResult(new DicomNActionResponse(request, DicomStatus.NoSuchSOPClass));
#else
                            return new DicomNActionResponse(request, DicomStatus.NoSuchSOPClass);
#endif
                        }
                    }

                    var printJob = new PrintJob(null, Printer, CallingAE, Logger)
                    {
                        SendNEventReport = _sendEventReports
                    };
                    printJob.StatusUpdate += OnPrintJobStatusUpdate;

                    printJob.Print(filmBoxList);

                    if (printJob.Error == null)
                    {

                        var result = new DicomDataset
                        {
                            new DicomSequence(DicomTag.ReferencedPrintJobSequenceRETIRED,
                            new DicomDataset(new DicomUniqueIdentifier(DicomTag.ReferencedSOPClassUID, DicomUID.PrintJobSOPClass)),
                            new DicomDataset(new DicomUniqueIdentifier(DicomTag.ReferencedSOPInstanceUID, printJob.SOPInstanceUID)))
                        };

                        var response = new DicomNActionResponse(request, DicomStatus.Success);
                        response.Command.AddOrUpdate(DicomTag.AffectedSOPInstanceUID, request.SOPInstanceUID);
                        response.Dataset = result;

#if FellowOakDicom5
                        return Task.FromResult(response);
#else
                        return response;
#endif
                    }
                    else
                    {
                        throw printJob.Error;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(
                        "Error occured during N-ACTION {0} for SOP class {1} and instance {2}",
                        request.ActionTypeID,
                        request.SOPClassUID.UID,
                        request.SOPInstanceUID.UID);
                    Logger.Error(ex.Message);
#if FellowOakDicom5
                    return Task.FromResult(new DicomNActionResponse(request, DicomStatus.ProcessingFailure));
#else
                    return new DicomNActionResponse(request, DicomStatus.ProcessingFailure);
#endif
                }
            }
        }

        private void OnPrintJobStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            var printJob = sender as PrintJob;
            if (printJob.SendNEventReport)
            {
                var reportRequest = new DicomNEventReportRequest(
                    printJob.SOPClassUID,
                    printJob.SOPInstanceUID,
                    e.EventTypeId);
                var ds = new DicomDataset
            {
               { DicomTag.ExecutionStatusInfo, e.ExecutionStatusInfo },
               { DicomTag.FilmSessionLabel, e.FilmSessionLabel },
               { DicomTag.PrinterName, e.PrinterName }
            };

                reportRequest.Dataset = ds;
                SendRequestAsync(reportRequest).Wait();
            }
        }

        #endregion

        #region IDicomNServiceProvider Members

#if FellowOakDicom5
        public Task<DicomNEventReportResponse> OnNEventReportRequestAsync(DicomNEventReportRequest request)
        {
            throw new NotImplementedException();
        }
#else
        public DicomNEventReportResponse OnNEventReportRequest(DicomNEventReportRequest request)
        {
            throw new NotImplementedException();
        }
#endif

        #endregion

        public void Clean()
        {
            //delete the current active print job and film sessions
            lock (_synchRoot)
            {
                _filmSession = null;
                _printJobList.Clear();
            }
        }
    }
}

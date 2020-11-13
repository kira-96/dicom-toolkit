namespace SimpleDICOMToolkit.Client
{
#if FellowOakDicom5
    using FellowOakDicom;
    using FellowOakDicom.Network;
    using FellowOakDicom.Network.Client;
#else
    using Dicom;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
#endif
    using StyletIoC;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Logging;

    public class WorklistSCU : IWorklistSCU
    {
        private const string IN_PROGRESS = "IN PROGRESS";
        private const string DISCONTINUED = "DISCONTINUED";
        private const string COMPLETED = "COMPLETED";

        private readonly ILoggerService Logger;

        private IEnumerable<DicomDataset> worklistItems;

        public WorklistSCU([Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            Logger = loggerService;
        }

        /// <summary>
        /// 参考
        /// https://github.com/fo-dicom/fo-dicom-samples/blob/master/Desktop/Worklist%20SCU/Program.cs
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="modality">Modality</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Dataset</returns>
        public async ValueTask<IEnumerable<DicomDataset>> GetAllItemsFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null, CancellationToken cancellationToken = default)
        {
            List<DicomDataset> worklistItems = new List<DicomDataset>();

            DicomCFindRequest worklistRequest = RequestFactory.CreateWorklistQuery(null, null, localAET, null, modality
                //, new DicomDateRange(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59))  // 时间限制：当天 00:00:00 ~ 23:59:59
                );

            worklistRequest.OnResponseReceived += (request, response) => 
            {
                if (!response.HasDataset)
                {
                    if (response.Status == DicomStatus.Success)
                        Logger.Debug("Worklist response END.");
                    else
                        Logger.Debug("Worklist response has [NO DATASET].");

                    return;
                }

                if (response.Status != DicomStatus.Success && 
                    response.Status != DicomStatus.Pending && 
                    response.Status != DicomStatus.QueryRetrieveOptionalKeysNotSupported)
                {
                    Logger.Error("Worklist response error - [{0}]", response.Status);
                    return;
                }

                worklistItems.Add(response.Dataset);
            };

#if FellowOakDicom5
            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);
#else
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
#endif

            await client.AddRequestAsync(worklistRequest);
            await client.SendAsync(cancellationToken);

            return worklistItems;
        }

        public async ValueTask<IEnumerable<SimpleWorklistResult>> GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null, Encoding fallbackEncoding = null, CancellationToken cancellationToken = default)
        {
            worklistItems = await GetAllItemsFromWorklistAsync(serverIp, serverPort, serverAET, localAET, modality, cancellationToken);

            List<SimpleWorklistResult> worklistResults = new List<SimpleWorklistResult>();

            foreach (DicomDataset item in worklistItems)
            {
                worklistResults.Add(GetWorklistResultFromDataset(item, fallbackEncoding));
            }

            return worklistResults;
        }

        public async ValueTask GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, ICollection<SimpleWorklistResult> worklistResults, string modality = null, Encoding fallbackEncoding = null, CancellationToken cancellationToken = default)
        {
            if (worklistResults == null)
            {
                throw new ArgumentException("Worklist results should not null.", nameof(worklistResults));
            }

            worklistItems = new List<DicomDataset>();

            DicomCFindRequest worklistRequest = RequestFactory.CreateWorklistQuery(null, null, localAET, null, modality
                //, new DicomDateRange(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59))  // 时间限制：当天 00:00:00 ~ 23:59:59
                );

            worklistRequest.OnResponseReceived += (request, response) =>
            {
                if (!response.HasDataset)
                {
                    if (response.Status == DicomStatus.Success)
                        Logger.Debug("Worklist response END.");
                    else
                        Logger.Debug("Worklist response has [NO DATASET].");

                    return;
                }

                if (response.Status != DicomStatus.Success &&
                    response.Status != DicomStatus.Pending &&
                    response.Status != DicomStatus.QueryRetrieveOptionalKeysNotSupported)
                {
                    Logger.Error("Worklist response error - [{0}]", response.Status);
                    return;
                }

                (worklistItems as List<DicomDataset>).Add(response.Dataset);
                worklistResults.Add(GetWorklistResultFromDataset(response.Dataset, fallbackEncoding));
            };

#if FellowOakDicom5
            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);
#else
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
#endif

            await client.AddRequestAsync(worklistRequest);
            await client.SendAsync(cancellationToken);
        }

        public async ValueTask<(DicomUID affectedInstanceUid, string studyInstanceUid, bool result)> SendMppsInProgressAsync(string serverIp, int serverPort, string serverAET, string localAET, DicomDataset worklistItem)
        {
            DicomSequence procedureStepSeq = worklistItem.GetSequence(DicomTag.ScheduledProcedureStepSequence);
            // A worklistitem may have a list of scheduledprocedureSteps.
            // For each of them you have to send separate MPPS InProgress- and Completed-messages.
            // there in this example we will only send for the first procedure step
            DicomDataset procedureStep = procedureStepSeq.First();

            // get study instance UID from MWL query resault
            string studyInstanceUID = worklistItem.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, DicomUIDGenerator.GenerateDerivedFromUUID().UID);

            DicomDataset dataset = new DicomDataset
            {
                { DicomTag.PatientName, worklistItem.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty) },
                { DicomTag.PatientID, worklistItem.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty) },
                { DicomTag.PatientBirthDate, worklistItem.GetSingleValueOrDefault(DicomTag.PatientBirthDate, string.Empty) },
                { DicomTag.PatientSex, worklistItem.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty) },
                { DicomTag.StudyID, worklistItem.GetSingleValueOrDefault(DicomTag.StudyID, string.Empty) },
                { DicomTag.StudyInstanceUID, studyInstanceUID },

                { DicomTag.PerformedProcedureStepID, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepID, string.Empty) },
                { DicomTag.PerformedProcedureStepDescription, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepID, string.Empty) },

                // set status
                { DicomTag.PerformedProcedureStepStatus, IN_PROGRESS },
                { DicomTag.PerformedProcedureTypeDescription, string.Empty },

                { DicomTag.PerformedProcedureStepStartDate, DateTime.Now },
                { DicomTag.PerformedProcedureStepStartTime, DateTime.Now },
                { DicomTag.PerformedProcedureStepEndDate, string.Empty },
                { DicomTag.PerformedProcedureStepEndTime, string.Empty },
                { DicomTag.PerformedLocation, string.Empty },
                { DicomTag.PerformedStationAETitle, localAET },
                { DicomTag.PerformedStationName, localAET },
                // get modality from MWL query result
                { DicomTag.Modality, procedureStep.GetSingleValueOrDefault(DicomTag.Modality, string.Empty) },
                { DicomTag.PerformedProtocolCodeSequence, new DicomDataset() },
                { DicomTag.ProcedureCodeSequence, new DicomDataset() },
                { DicomTag.ReferencedPatientSequence, new DicomDataset() }
            };
            // set Attribute Sequence data
            DicomDataset content = new DicomDataset
            {
                { DicomTag.AccessionNumber, worklistItem.GetSingleValueOrDefault(DicomTag.AccessionNumber, string.Empty) },
                { DicomTag.StudyInstanceUID, studyInstanceUID },
                { DicomTag.ReferencedStudySequence, new DicomDataset() },
                { DicomTag.RequestedProcedureID, worklistItem.GetSingleValueOrDefault(DicomTag.RequestedProcedureID, string.Empty) },
                { DicomTag.RequestedProcedureDescription, worklistItem.GetSingleValueOrDefault(DicomTag.RequestedProcedureDescription, string.Empty) },
                { DicomTag.ScheduledProcedureStepID, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepID, string.Empty) },
                { DicomTag.ScheduledProcedureStepDescription, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepDescription, string.Empty) },
                { DicomTag.ScheduledProtocolCodeSequence, new DicomDataset() }
            };

            DicomSequence attr_Sequence = new DicomSequence(DicomTag.ScheduledStepAttributesSequence, content);//"Scheduled Step Attribute Sequence"
            dataset.Add(attr_Sequence);

            dataset.Add(DicomTag.PerformedSeriesSequence, new DicomDataset());

            // create an unique UID as the effectedinstamceUid, this id will be needed for the N-SET also
            DicomUID effectedinstamceUid = DicomUID.Generate();

            DicomNCreateRequest dicomStartRequest = new DicomNCreateRequest(DicomUID.ModalityPerformedProcedureStepSOPClass, effectedinstamceUid)
            {
                Dataset = dataset
            };

            bool result = false;

            dicomStartRequest.OnResponseReceived += (req, res) =>
            {
                if (res != null)
                {
                    if (res.Status == DicomStatus.Success)
                    {
                        Logger.Debug("Set study [{0}] [In Progress] Success.", studyInstanceUID);

                        result = true;
                    }
                    else
                    {
                        Logger.Warn("Set study [{0}] [In Progress] Failed. [{1}]", studyInstanceUID, res.Status);
                    }
                }
            };

#if FellowOakDicom5
            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);
#else
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
#endif
            await client.AddRequestAsync(dicomStartRequest);
            await client.SendAsync();

            return (effectedinstamceUid, studyInstanceUID, result);
        }

        public async ValueTask<bool> SendMppsCompletedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem)
        {
            DicomSequence procedureStepSq = worklistItem.GetSequence(DicomTag.ScheduledProcedureStepSequence);
            // A worklistitem may have a list of scheduledprocedureSteps.
            // For each of them you have to send separate MPPS InProgress- and Completed-messages.
            // there in this example we will only send for the first procedure step
            DicomDataset procedureStep = procedureStepSq.First();

            // data
            DicomDataset dataset = new DicomDataset
            {
                { DicomTag.StudyInstanceUID, studyInstanceUid },
                { DicomTag.PerformedProcedureStepEndDate, DateTime.Now },
                { DicomTag.PerformedProcedureStepEndTime, DateTime.Now },
                { DicomTag.PerformedProcedureStepStatus, COMPLETED },
                { DicomTag.PerformedProcedureStepDescription, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepID, string.Empty) },
                { DicomTag.PerformedProcedureTypeDescription, string.Empty },

                { DicomTag.PerformedProtocolCodeSequence, new DicomDataset() },
                { DicomTag.ProcedureCodeSequence, new DicomDataset() },

                // dose and reports
                { DicomTag.ImageAndFluoroscopyAreaDoseProduct, 0.0m }, // if there has bee sone dose while examination
                { DicomTag.CommentsOnRadiationDose, string.Empty }, // a free text that contains all dose parameters

                { DicomTag.PerformedSeriesSequence, new DicomDataset() 
                {
                    { DicomTag.RetrieveAETitle, string.Empty }, // the aetitle of the archive where the images have been sent to
                    { DicomTag.SeriesDescription, "series 1" },
                    { DicomTag.PerformingPhysicianName, string.Empty },
                    { DicomTag.OperatorsName, string.Empty },
                    { DicomTag.ProtocolName, "protocol 1" },
                    { DicomTag.SeriesInstanceUID, DicomUID.Generate() },
                    { DicomTag.ReferencedImageSequence, new DicomDataset()
                    {
                        { DicomTag.ReferencedSOPClassUID, DicomUID.SecondaryCaptureImageStorage },
                        { DicomTag.ReferencedSOPInstanceUID, DicomUID.Generate() }
                    }},
                }}
            };

            //// images created
            //DicomSequence performedSeriesSq = new DicomSequence(DicomTag.PerformedSeriesSequence);
            //// iterate all Series that have been created while examination
            //DicomDataset serie = new DicomDataset()
            //{
            //    { DicomTag.RetrieveAETitle, string.Empty }, // the aetitle of the archive where the images have been sent to
            //    { DicomTag.SeriesDescription, "serie 1" },
            //    { DicomTag.PerformingPhysicianName, string.Empty },
            //    { DicomTag.OperatorsName, string.Empty },
            //    { DicomTag.ProtocolName, string.Empty },
            //    { DicomTag.SeriesInstanceUID, DicomUID.Generate() }
            //};
            //DicomSequence refImagesInSerie = new DicomSequence(DicomTag.ReferencedImageSequence);
            //// iterate all images in the serie
            //DicomDataset image = new DicomDataset()
            //{
            //    { DicomTag.ReferencedSOPClassUID, DicomUID.SecondaryCaptureImageStorage },
            //    { DicomTag.ReferencedSOPInstanceUID, DicomUID.Generate() }
            //};
            //refImagesInSerie.Items.Add(image);
            //serie.Add(refImagesInSerie);
            //performedSeriesSq.Items.Add(serie);
            //dataset.Add(performedSeriesSq);

            bool result = false;

            DicomNSetRequest dicomFinished = new DicomNSetRequest(DicomUID.ModalityPerformedProcedureStepSOPClass, affectedInstanceUid)
            {
                Dataset = dataset,
                OnResponseReceived = (req, res) =>
                {
                    if (res != null)
                    {
                        if (res.Status == DicomStatus.Success)
                        {
                            result = true;
                            Logger.Debug("Set study [{0}] [Complete] Success.", studyInstanceUid);
                        }
                        else
                        {
                            Logger.Warn("Set study [{0}] [Complete] Failed. [{1}]", studyInstanceUid, res.Status);
                        }
                    }
                }
            };

#if FellowOakDicom5
            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);
#else
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
#endif

            await client.AddRequestAsync(dicomFinished);
            await client.SendAsync();

            return result;
        }

        public async ValueTask<bool> SendMppsDiscontinuedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem)
        {
            DicomSequence procedureStepSq = worklistItem.GetSequence(DicomTag.ScheduledProcedureStepSequence);
            // A worklistitem may have a list of scheduledprocedureSteps.
            // For each of them you have to send separate MPPS InProgress- and Completed-messages.
            // there in this example we will only send for the first procedure step
            DicomDataset procedureStep = procedureStepSq.First();

            DicomDataset dataset = new DicomDataset()
            {
                { DicomTag.StudyInstanceUID, studyInstanceUid },
                { DicomTag.PerformedProcedureStepEndDate, DateTime.Now },
                { DicomTag.PerformedProcedureStepEndTime, DateTime.Now },
                { DicomTag.PerformedProcedureStepStatus, DISCONTINUED },
                { DicomTag.PerformedProcedureStepDescription, procedureStep.GetSingleValueOrDefault(DicomTag.ScheduledProcedureStepID, string.Empty) },
                { DicomTag.PerformedProcedureTypeDescription, string.Empty },

                { DicomTag.PerformedProtocolCodeSequence, new DicomDataset() },
                { DicomTag.ProcedureCodeSequence, new DicomDataset() },

                { DicomTag.PerformedSeriesSequence, new DicomDataset()
                {
                    { DicomTag.RetrieveAETitle, string.Empty }, // the aetitle of the archive where the images have been sent to
                    { DicomTag.SeriesDescription, "series 1" },
                    { DicomTag.PerformingPhysicianName, string.Empty },
                    { DicomTag.OperatorsName, string.Empty },
                    { DicomTag.ProtocolName, "protocol 1" },
                    { DicomTag.SeriesInstanceUID, DicomUID.Generate() },
                    { DicomTag.ReferencedImageSequence, new DicomDataset()
                    {
                        { DicomTag.ReferencedSOPClassUID, DicomUID.SecondaryCaptureImageStorage },
                        { DicomTag.ReferencedSOPInstanceUID, DicomUID.Generate() }
                    }},
                }}
            };

            bool result = false;

            DicomNSetRequest dicomAbort = new DicomNSetRequest(DicomUID.ModalityPerformedProcedureStepSOPClass, affectedInstanceUid)
            {
                Dataset = dataset,
                OnResponseReceived = (req, res) =>
                {
                    if (res != null)
                    {
                        if (res.Status == DicomStatus.Success)
                        {
                            result = true;
                            Logger.Debug("Set study [{0}] [Discontinued] Success.", studyInstanceUid);
                        }
                        else
                        {
                            Logger.Warn("Set study [{0}] [Discontinued] Failed. [{1}]", studyInstanceUid, res.Status);
                        }
                    }
                }
            };

#if FellowOakDicom5
            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);
#else
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
#endif

            await client.AddRequestAsync(dicomAbort);
            await client.SendAsync();

            return result;
        }

        public DicomDataset GetWorklistItemByStudyUid(string sid)
        {
            if (string.IsNullOrEmpty(sid))
                return null;

            foreach (DicomDataset item in worklistItems)
            {
                string stuId = item.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

                if (stuId == sid)
                {
                    return item;
                }
            }

            return null;
        }

        private SimpleWorklistResult GetWorklistResultFromDataset(DicomDataset dataset, Encoding fallbackEncoding = null)
        {
            Encoding encoding = fallbackEncoding ?? Encoding.UTF8;

            if (dataset.TryGetString(DicomTag.SpecificCharacterSet, out string charset))
            {
                encoding = DicomEncoding.GetEncoding(charset);
            }

            string name = dataset.Contains(DicomTag.PatientName) ? encoding.GetString(dataset.GetValues<byte>(DicomTag.PatientName)) : string.Empty;
            string sex = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, string.Empty);
            string age = dataset.GetSingleValueOrDefault(DicomTag.PatientAge, string.Empty);
            string patId = dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty);
            string studyUid = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

            return new SimpleWorklistResult(name, sex, age, patId, studyUid);
        }
    }
}

// https://github.com/fo-dicom/fo-dicom-samples/blob/master/Desktop/C-Store%20SCP/Program.cs#L36

// Copyright (c) 2012-2020 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

namespace SimpleDICOMToolkit.Server
{
    using FellowOakDicom;
    using FellowOakDicom.Imaging.Codec;
    using FellowOakDicom.Log;
    using FellowOakDicom.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class StoreService : DicomService, IDicomServiceProvider, IDicomCEchoProvider, IDicomCStoreProvider, IDicomNServiceProvider
    {
        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
        {
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes = new DicomTransferSyntax[]
        {
            // Lossless
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEGProcess14SV1,
            DicomTransferSyntax.JPEGProcess14,
            DicomTransferSyntax.RLELossless,
            // Lossy
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.JPEGProcess1,
            DicomTransferSyntax.JPEGProcess2_4,
            // Uncompressed
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private readonly List<string> receivedFiles;

        public StoreService(INetworkStream stream, Encoding fallbackEncoding, ILogger logger, ILogManager logManager, INetworkManager networkManager, ITranscoderManager transcoderManager)
            : base(stream, fallbackEncoding, logger, logManager, networkManager, transcoderManager)
        {
            receivedFiles = new List<string>();
        }

        public void OnConnectionClosed(Exception exception)
        {
            Logger.Info("C-STORE Connection closed");
            if (receivedFiles.Any())
            {
                StoreServer.Default.OnFilesSaved?.Invoke(receivedFiles);
                receivedFiles.Clear();
            }
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            Logger.Error("Received abort from {0}, reason is {1}", source, reason);
            if (receivedFiles.Any())
            {
                StoreServer.Default.OnFilesSaved?.Invoke(receivedFiles);
                receivedFiles.Clear();
            }
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            if (association.CalledAE != StoreServer.Default.AETitle)
            {
                return SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    DicomRejectReason.CalledAENotRecognized);
            }

            foreach (DicomPresentationContext pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification)
                { pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes); }
                else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                { pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes); }
                else
                { }
            }

            return SendAssociationAcceptAsync(association);
        }

        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
        }

        public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
        {
            string studyUid = request.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            string instUid = request.SOPInstanceUID.UID;

            // 设置中应该支持设置存储目录
            string path = Path.GetFullPath(StoreServer.Default.DcmDirPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, studyUid);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, instUid) + ".dcm";

            request.File.Save(path);

            receivedFiles.Add(path);

            return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.Success));
        }

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
        {
            return Task.CompletedTask;
        }

        #region N-Service

        public Task<DicomNActionResponse> OnNActionRequestAsync(DicomNActionRequest request)
        {
            Logger.Info("Received N-Action request, Action type ID {0}.", request.ActionTypeID);
            return Task.FromResult(new DicomNActionResponse(request, DicomStatus.Success));
        }

        public Task<DicomNCreateResponse> OnNCreateRequestAsync(DicomNCreateRequest request)
        {
            Logger.Info("Received N-Create request, message ID {0}.", request.MessageID);
            return Task.FromResult(new DicomNCreateResponse(request, DicomStatus.Success));
        }

        public Task<DicomNDeleteResponse> OnNDeleteRequestAsync(DicomNDeleteRequest request)
        {
            Logger.Info("Received N-Delete request, message ID {0}.", request.MessageID);
            return Task.FromResult(new DicomNDeleteResponse(request, DicomStatus.Success));
        }

        public Task<DicomNEventReportResponse> OnNEventReportRequestAsync(DicomNEventReportRequest request)
        {
            Logger.Info("Received N-EventReport request, event type ID {0}.", request.EventTypeID);
            return Task.FromResult(new DicomNEventReportResponse(request, DicomStatus.Success));
        }

        public Task<DicomNGetResponse> OnNGetRequestAsync(DicomNGetRequest request)
        {
            Logger.Info("Received N-Get request, message ID {0}.", request.MessageID);
            return Task.FromResult(new DicomNGetResponse(request, DicomStatus.Success));
        }

        public Task<DicomNSetResponse> OnNSetRequestAsync(DicomNSetRequest request)
        {
            Logger.Info("Received N-Set request, message ID {0}.", request.MessageID);
            return Task.FromResult(new DicomNSetResponse(request, DicomStatus.Success));
        }

        #endregion
    }
}

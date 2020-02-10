namespace SimpleDICOMToolkit.Server
{
    using Dicom;
    using Dicom.Log;
    using Dicom.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class CStoreSCP : DicomService, IDicomServiceProvider, IDicomCEchoProvider, IDicomCStoreProvider
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

        private readonly List<string> _receivedFiles;

        public Action<IList<string>> OnFilesSaved;

        public string LocalAET { get; set; }

        public CStoreSCP(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
        {
            _receivedFiles = new List<string>();
        }

        public void OnConnectionClosed(Exception exception)
        {
            OnFilesSaved?.Invoke(_receivedFiles);
            _receivedFiles.Clear();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            OnFilesSaved?.Invoke(_receivedFiles);
            _receivedFiles.Clear();
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            if (association.CalledAE != CStoreServer.Default.AETitle)
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

        public DicomCEchoResponse OnCEchoRequest(DicomCEchoRequest request)
        {
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            string studyUid = request.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            string instUid = request.SOPInstanceUID.UID;

            // 设置中应该支持设置存储目录
            string path = Path.GetFullPath(CStoreServer.Default.DcmDirPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, studyUid);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, instUid) + ".dcm";

            request.File.Save(path);

            _receivedFiles.Add(path);

            return new DicomCStoreResponse(request, DicomStatus.Success);
        }

        public void OnCStoreRequestException(string tempFileName, Exception e)
        {
            // let library handle logging and error response
        }
    }
}

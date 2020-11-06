using Dicom.Network;
using System.Collections.Generic;
using System.Text;
using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Server
{
    public class WorklistServer
    {
        private static readonly object locker = new object();

        private static WorklistServer instance = null;

        private IDicomServer defaultServer = null;

        public IMppsSource MppsSource { get; private set; } = null;

        // must set
        public IEnumerable<IWorklistItem> WorklistItems { get; set; } = null;

        public static WorklistServer Default
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new WorklistServer();
                        }
                    }
                }

                return instance;
            }
        }

        public string AETitle { get; private set; } = "";

        private WorklistServer()
        {}

        public bool IsListening()
        {
            if (defaultServer == null)
                return false;

            return defaultServer.IsListening;
        }

        public bool CreateServer(int port, string serverAET, Encoding fallbackEncoding = null)
        {
            if (IsListening())
                return true;

            AETitle = serverAET;

            defaultServer = DicomServer.Create<WorklistService>(port, null, null, fallbackEncoding);
            MppsSource = new MppsHandler(WorklistItems, defaultServer.Logger);

            return IsListening();
        }

        public void StopServer()
        {
            if (IsListening())
            {
                defaultServer.Stop();
                defaultServer.Dispose();
            }
        }
    }
}

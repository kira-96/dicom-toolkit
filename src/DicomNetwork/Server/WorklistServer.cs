using Dicom.Network;

namespace SimpleDICOMToolkit.Server
{
    public class WorklistServer
    {
        private static readonly object locker = new object();

        private static WorklistServer instance = null;

        private IDicomServer defaultServer = null;

        // must set
        public WorklistItemsSource ItemsSource { get; set; } = null;

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

        public bool CreateServer(int port, string serverAET)
        {
            if (IsListening())
                return true;

            AETitle = serverAET;

            defaultServer = DicomServer.Create<WorklistService>(port);

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

using Dicom.Network;

namespace SimpleDICOMToolkit.Server
{
    public class PrintServer
    {
        private static readonly object locker = new object();

        private static PrintServer instance = null;

        private IDicomServer defaultServer = null;

        public static PrintServer Default
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new PrintServer();
                        }
                    }
                }

                return instance;
            }
        }

        public string AETitle { get; private set; } = "";

        public Printer Printer { get; private set; }

        private PrintServer()
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

            defaultServer = DicomServer.Create<PrintSCP>(port);

            Printer = new Printer(serverAET);

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

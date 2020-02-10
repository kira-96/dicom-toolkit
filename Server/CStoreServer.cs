using Dicom.Network;

namespace SimpleDICOMToolkit.Server
{
    public class CStoreServer
    {
        private static readonly object locker = new object();

        private static CStoreServer instance = null;

        private IDicomServer defaultServer = null;

        public static CStoreServer Default
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new CStoreServer();
                        }
                    }
                }

                return instance;
            }
        }

        public string AETitle { get; private set; } = "";

        public string DcmDirPath { get; private set; } = "DICM";

        private CStoreServer()
        {}

        public bool IsListening()
        {
            if (defaultServer == null)
                return false;

            return defaultServer.IsListening;
        }

        public bool CreateServer(int port, string serverAET, string fileSaveDir = "")
        {
            if (IsListening())
                return true;

            AETitle = serverAET;

            if (!string.IsNullOrEmpty(fileSaveDir))
            {
                DcmDirPath = fileSaveDir;

                // 如果文件夹不存在就创建
                if (!System.IO.Directory.Exists(fileSaveDir))
                {
                    System.IO.Directory.CreateDirectory(fileSaveDir);
                }
            }

            defaultServer = DicomServer.Create<CStoreSCP>(port);

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

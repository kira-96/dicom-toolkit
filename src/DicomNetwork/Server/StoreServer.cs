#if FellowOakDicom5
using FellowOakDicom.Network;
#else
using Dicom.Network;
#endif
using System;
using System.Collections.Generic;

namespace SimpleDICOMToolkit.Server
{
    public class StoreServer
    {
        private static readonly object locker = new object();

        private static StoreServer instance = null;

        private IDicomServer defaultServer = null;

        public static StoreServer Default
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new StoreServer();
                        }
                    }
                }

                return instance;
            }
        }

        public Action<IList<string>> OnFilesSaved;

        public string AETitle { get; private set; } = "";

        public string DcmDirPath { get; private set; } = "DICOM";

        private StoreServer()
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

#if FellowOakDicom5
            defaultServer = DicomServerFactory.Create<StoreService>(port);
#else
            defaultServer = DicomServer.Create<StoreService>(port);
#endif

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

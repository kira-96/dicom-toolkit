using FellowOakDicom.Network;
using System.Drawing.Printing;

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

        public string PrinterName { get; set; } = "Microsoft XPS Document Writer";  // XPS - Windows 系统都有

        public bool IsSaveToImage => PrinterName == "PNG";

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

            defaultServer = DicomServerFactory.Create<PrintService>(port);

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

        public PrinterSettings GetPrinterSettings(string fileName)
        {
            if (PrinterName == "Microsoft Print to PDF")
            {
                return new PrinterSettings()
                {
                    PrinterName = PrinterName,
                    PrintToFile = true,
                    PrintFileName = fileName + ".pdf"
                };
            }
            else if (PrinterName == "Microsoft XPS Document Writer")
            {
                return new PrinterSettings()
                {
                    PrinterName = PrinterName,
                    PrintToFile = true,
                    PrintFileName = fileName + ".xps"
                };
            }
            else
            {
                return new PrinterSettings()
                {
                    PrinterName = PrinterName
                };
            }
        }
    }
}

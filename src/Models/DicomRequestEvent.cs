namespace SimpleDICOMToolkit.Models
{
    public class DicomRequestEvent
    {
        public string ServerIP { get; }

        public int ServerPort { get; }

        public string ServerAET { get; }

        public string LocalAET { get; }

        public string Modality { get; }

        public DicomRequestEvent(
            string ip,
            int port,
            string serverAET,
            string localAET,
            string modality = null)
        {
            ServerIP = ip;
            ServerPort = port;
            ServerAET = serverAET;
            LocalAET = localAET;
            Modality = modality;
        }

        public override string ToString()
        {
            return string.Format("SCP: {0}:{1} {2}, SCU AET: {3}", ServerIP, ServerPort, ServerAET, LocalAET);
        }
    }
}

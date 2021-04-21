namespace SimpleDICOMToolkit.Models
{
    public class DicomServiceEvent
    {
        public string ServerIP { get; }

        public int ServerPort { get; }

        public string LocalAET { get; }

        public DicomServiceEvent(string ip, int port, string aet)
        {
            ServerIP = ip;
            ServerPort = port;
            LocalAET = aet;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", LocalAET, ServerPort);
        }
    }
}

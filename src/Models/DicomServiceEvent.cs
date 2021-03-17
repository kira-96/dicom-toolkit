namespace SimpleDICOMToolkit.Models
{
    public class DicomServiceEvent
    {
        public int ServerPort { get; }

        public string LocalAET { get; }

        public DicomServiceEvent(int port, string aet)
        {
            ServerPort = port;
            LocalAET = aet;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", LocalAET, ServerPort);
        }
    }
}

namespace SimpleDICOMToolkit.Models
{
    public class ServerMessageItem
    {
        public int ServerPort { get; private set; }

        public string LocalAET { get; private set; }

        public ServerMessageItem(int port, string aet)
        {
            ServerPort = port;
            LocalAET = aet;
        }
    }
}

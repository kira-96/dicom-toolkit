namespace SimpleDICOMToolkit.Models
{
    public class CStoreServerItem
    {

        public int ServerPort { get; private set; }

        public string LocalAET { get; private set; }

        public CStoreServerItem(int port, string aet)
        {
            ServerPort = port;
            LocalAET = aet;
        }
    }
}

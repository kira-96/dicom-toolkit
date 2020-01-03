namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICStoreSCU
    {
        Task StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<CStoreItem> dcmFiles);
    }
}

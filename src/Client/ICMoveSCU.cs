namespace SimpleDICOMToolkit.Client
{
    using System.Threading.Tasks;
    public interface ICMoveSCU
    {
        Task MoveImageAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid);
    }
}

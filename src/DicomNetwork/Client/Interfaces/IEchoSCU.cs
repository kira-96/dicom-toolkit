namespace SimpleDICOMToolkit.Client
{
    using System.Threading.Tasks;

    public interface IEchoSCU
    {
        /// <summary>
        /// C-ECHO request
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        /// <param name="serverAET"></param>
        /// <param name="localAET"></param>
        /// <returns>true if success</returns>
        Task<bool> Echo(string serverIp, int serverPort, string serverAET, string localAET);
    }
}

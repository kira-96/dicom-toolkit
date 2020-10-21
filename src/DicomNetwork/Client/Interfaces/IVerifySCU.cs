namespace SimpleDICOMToolkit.Client
{
    using System.Threading.Tasks;

    public interface IVerifySCU
    {
        /// <summary>
        /// Verify remote SCP
        /// Send C-ECHO request
        /// </summary>
        /// <param name="serverIp">Server IP</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="serverAET">Server AET</param>
        /// <param name="localAET">Local AET</param>
        /// <returns>True if success</returns>
        Task<bool> VerifyAsync(string serverIp, int serverPort, string serverAET, string localAET);
    }
}

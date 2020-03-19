namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICStoreSCU
    {
        /// <summary>
        /// 发送图像文件到远程服务器
        /// </summary>
        /// <param name="serverIp">Server IP Addr</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="serverAET">Server AE Title</param>
        /// <param name="localAET">Client AE Title</param>
        /// <param name="dcmFiles">files path</param>
        /// <returns>Task</returns>
        Task StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<CStoreItem> dcmFiles);
    }
}

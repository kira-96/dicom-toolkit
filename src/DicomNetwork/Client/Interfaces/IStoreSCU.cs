namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;

    public interface IStoreSCU
    {
        /// <summary>
        /// 发送图像文件到远程服务器
        /// </summary>
        /// <param name="serverIp">Server IP Addr</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="serverAET">Server AE Title</param>
        /// <param name="localAET">Client AE Title</param>
        /// <param name="dcmFiles">files path</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>ValueTask</returns>
        ValueTask StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<IStoreItem> dcmFiles, CancellationToken cancellationToken = default);
    }
}

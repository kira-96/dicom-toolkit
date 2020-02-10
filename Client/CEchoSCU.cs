namespace SimpleDICOMToolkit.Client
{
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Threading.Tasks;

    public class CEchoSCU : ICEchoSCU
    {
        /// <summary>
        /// 测试请求
        /// </summary>
        /// <param name="serverIp">Server IP Addr</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="serverAET">Server AE Title</param>
        /// <param name="localAET">Client AE Title</param>
        /// <returns>true if success</returns>
        public async Task<bool> Echo(string serverIp, int serverPort, string serverAET, string localAET)
        {
            bool echoResult = false;

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            client.NegotiateAsyncOps();

            DicomCEchoRequest request = new DicomCEchoRequest()
            {
                OnResponseReceived = (req, res) =>
                {
                    if (res.Status == DicomStatus.Success)
                        echoResult = true;
                }
            };

            await client.AddRequestAsync(request);

            try
            {
                await client.SendAsync();
            }
            catch (System.Exception)
            {
                return false;
            }

            return echoResult;
        }
    }
}

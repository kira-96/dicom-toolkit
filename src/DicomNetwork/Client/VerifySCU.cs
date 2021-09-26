namespace SimpleDICOMToolkit.Client
{
    using FellowOakDicom.Network;
    using FellowOakDicom.Network.Client;
    using StyletIoC;
    using System.Threading.Tasks;
    using Logging;

    public class VerifySCU : IVerifySCU
    {
        private ILoggerService loggerService;

        public VerifySCU([Inject("filelogger")] ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        /// <summary>
        /// 测试请求
        /// </summary>
        /// <param name="serverIp">Server IP Addr</param>
        /// <param name="serverPort">Server Port</param>
        /// <param name="serverAET">Server AE Title</param>
        /// <param name="localAET">Client AE Title</param>
        /// <returns>true if success</returns>
        public async ValueTask<bool> VerifyAsync(string serverIp, int serverPort, string serverAET, string localAET)
        {
            bool echoResult = false;

            DicomCEchoRequest request = new DicomCEchoRequest()
            {
                OnResponseReceived = (req, res) =>
                {
                    if (res.Status == DicomStatus.Success)
                        echoResult = true;
                }
            };

            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);

            await client.AddRequestAsync(request);

            try
            {
                await client.SendAsync();
            }
            catch (System.Exception ex)
            {
                loggerService.Error(ex);
                return false;
            }

            return echoResult;
        }
    }
}

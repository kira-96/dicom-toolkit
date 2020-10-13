namespace SimpleDICOMToolkit.Client
{
    using StyletIoC;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Threading.Tasks;
    using Logging;

    public class EchoSCU : IEchoSCU
    {
        private ILoggerService loggerService;

        public EchoSCU([Inject("filelogger")] ILoggerService loggerService)
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
        public async Task<bool> Echo(string serverIp, int serverPort, string serverAET, string localAET)
        {
            bool echoResult = false;

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);

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
            catch (System.Exception ex)
            {
                loggerService.Error(ex);
                return false;
            }

            return echoResult;
        }
    }
}

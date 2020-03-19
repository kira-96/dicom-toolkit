namespace SimpleDICOMToolkit.Client
{
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Threading.Tasks;
    using Logging;

    public class CMoveSCU : ICMoveSCU
    {
        private readonly ILoggerService _logger = SimpleIoC.Get<ILoggerService>("filelogger");

        public async Task MoveImageAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid)
        {
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            client.NegotiateAsyncOps();

            DicomCMoveRequest request = new DicomCMoveRequest(serverAET, studyInstanceUid)
            {
                OnResponseReceived = (req, res) =>
                {
                    if (res.Status != DicomStatus.Success)
                    {
                        _logger.Error("C-MOVE failed. Study Instance UID - [{0}]", studyInstanceUid);
                    }
                }
            };

            await client.AddRequestAsync(request);
            await client.SendAsync();
        }
    }
}

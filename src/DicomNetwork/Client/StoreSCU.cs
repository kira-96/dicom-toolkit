namespace SimpleDICOMToolkit.Client
{
    using StyletIoC;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Logging;

    public class StoreSCU : IStoreSCU
    {
        private readonly ILoggerService Logger;

        public StoreSCU([Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            Logger = loggerService;
        }

        public async ValueTask StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<IStoreItem> items, CancellationToken cancellationToken = default)
        {
            List<DicomCStoreRequest> requests = new List<DicomCStoreRequest>();

            foreach (IStoreItem item in items)
            {
                DicomCStoreRequest request = new DicomCStoreRequest(item.File)
                {
                    OnResponseReceived = (req, res) =>
                    {
                        if (res.Status != DicomStatus.Success)
                        {
                            Logger.Error("C-STORE send failed. Instance UID - [{0}]", req.SOPInstanceUID);
                            item.Status = StoreItemStatus.Failed;
                        }
                        else
                        {
                            item.Status = StoreItemStatus.Success;
                        }
                    }
                };

                requests.Add(request);
            }

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);

            await client.AddRequestsAsync(requests);
            await client.SendAsync(cancellationToken);
        }
    }
}

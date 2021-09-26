﻿namespace SimpleDICOMToolkit.Client
{
    using FellowOakDicom.Network;
    using FellowOakDicom.Network.Client;
    using StyletIoC;
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

        public async ValueTask<int> StoreImageAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<IStoreItem> items, CancellationToken cancellationToken = default)
        {
            int errors = 0;
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
                            errors += 1;
                        }
                        else
                        {
                            item.Status = StoreItemStatus.Success;
                        }
                    }
                };

                requests.Add(request);
            }

            IDicomClient client = DicomClientFactory.Create(serverIp, serverPort, false, localAET, serverAET);

            await client.AddRequestsAsync(requests);
            await client.SendAsync(cancellationToken);

            return errors;
        }
    }
}

namespace SimpleDICOMToolkit.Client
{
    using Dicom;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Logging;

    public class QueryRetrieveSCU : IQueryRetrieveSCU
    {
        private readonly ILoggerService logger = SimpleIoC.Get<ILoggerService>("filelogger");

        public async Task<List<DicomDataset>> QueryPatients(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null)
        {
            List<DicomDataset> patients = new List<DicomDataset>();

            DicomCFindRequest request = RequestFactory.CreatePatientQuery(patientId, patientName);
            request.Dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 192");
            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status == DicomStatus.Success ||
                    res.Status == DicomStatus.Pending)
                {
                    if (res.HasDataset)
                    {
                        patients.Add(res.Dataset);
                    }
                    else
                    {
                        logger.Error("Query Patients response has no dataset.");
                    }
                }
                else
                {
                    logger.Error("Query Patients failure. Status - [{0}]", res.Status);
                }
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return patients;
        }

        public async Task<List<string>> QueryStudiesByPatientAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null, DicomDateRange studyDateTime = null)
        {
            List<string> studyUids = new List<string>();

            DicomCFindRequest request = RequestFactory.CreateStudyQuery(patientId, patientName, studyDateTime);
            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status == DicomStatus.Success ||
                    res.Status == DicomStatus.Pending)
                {
                    if (res.HasDataset && res.Dataset.Contains(DicomTag.StudyInstanceUID))
                    {
                        studyUids.Add(res.Dataset.GetString(DicomTag.StudyInstanceUID));
                    }
                    else
                    {
                        logger.Error("Query response dataset not contains [Study Instance UID].");
                    }
                }
                else
                {
                    logger.Error("Query Studies failure. Status - [{0}]", res.Status);
                }
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return studyUids;
        }

        public async Task<List<string>> QuerySeriesByStudyAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string modality = null)
        {
            List<string> seriesUids = new List<string>();

            DicomCFindRequest request = RequestFactory.CreateSeriesQuery(studyInstanceUid, modality);
            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status == DicomStatus.Success ||
                    res.Status == DicomStatus.Pending)
                {
                    if (res.HasDataset && res.Dataset.Contains(DicomTag.SeriesInstanceUID))
                    {
                        seriesUids.Add(res.Dataset.GetString(DicomTag.SeriesInstanceUID));
                    }
                    else
                    {
                        logger.Error("Query response dataset not contains [Series Instance UID].");
                    }
                }
                else
                {
                    logger.Error("Query Series failure. Status - [{0}]", res.Status);
                }
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return seriesUids;
        }

        public async Task<List<string>> QueryImagesByStudyAndSeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string modality = null)
        {
            List<string> sopUids = new List<string>();

            DicomCFindRequest request = RequestFactory.CreateImageQuery(studyInstanceUid, seriesInstanceUid, modality);
            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status == DicomStatus.Success ||
                    res.Status == DicomStatus.Pending)
                {
                    if (res.HasDataset && res.Dataset.Contains(DicomTag.SOPInstanceUID))
                    {
                        sopUids.Add(res.Dataset.GetString(DicomTag.SOPInstanceUID));
                    }
                    else
                    {
                        logger.Error("Query response dataset not contains [SOP Instance UID].");
                    }
                }
                else
                {
                    logger.Error("Query Images failure. Status - [{0}]", res.Status);
                }
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return sopUids;
        }

        public async Task<List<DicomDataset>> GetImagesBySeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid)
        {
            List<DicomDataset> imageDatasets = new List<DicomDataset>();

            DicomCGetRequest request = RequestFactory.CreateCGetBySeriesUID(studyInstanceUid, seriesInstanceUid);
            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            client.OnCStoreRequest += async (req) => 
            {
                if (req.HasDataset)
                {
                    imageDatasets.Add(req.Dataset);
                    return await Task.FromResult(new DicomCStoreResponse(req, DicomStatus.Success));
                }
                else
                {
                    logger.Error("C-STORE request has no dataset.");
                    return await Task.FromResult(new DicomCStoreResponse(req, DicomStatus.AttributeListError));
                }
            };
            // the client has to accept storage of the images. We know that the requested images are of SOP class Secondary capture,
            // so we add the Secondary capture to the additional presentation context
            // a more general approach would be to mace a cfind-request on image level and to read a list of distinct SOP classes of all
            // the images. these SOP classes shall be added here.
            var pcs = DicomPresentationContext.GetScpRolePresentationContextsFromStorageUids(
                DicomStorageCategory.Image,
                DicomTransferSyntax.ExplicitVRLittleEndian,
                DicomTransferSyntax.ImplicitVRLittleEndian,
                DicomTransferSyntax.ImplicitVRBigEndian);
            client.AdditionalPresentationContexts.AddRange(pcs);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return imageDatasets;
        }

        public async Task<bool?> MoveImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, string destAET, string studyInstanceUid, string seriesInstanceUid = null)
        {
            bool? success = null;

            DicomCMoveRequest request = string.IsNullOrEmpty(seriesInstanceUid) ?
                RequestFactory.CreateCMoveByStudyUID(destAET, studyInstanceUid) :
                RequestFactory.CreateCMoveBySeriesUID(destAET, studyInstanceUid, seriesInstanceUid);

            request.OnResponseReceived += (req, res) =>
            {
                if (res.Status.State == DicomState.Pending)
                {
                    logger.Info("Sending is in progress. please wait: " + res.Remaining.ToString());
                }
                else if (res.Status.State == DicomState.Success)
                {
                    logger.Info("Sending successfully finished");
                    success = true;
                }
                else if (res.Status.State == DicomState.Failure)
                {
                    logger.Info("Error sending datasets: " + res.Status.Description);
                    success = false;
                }
                logger.Debug("C-MOVE response status.");
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);
            await client.AddRequestAsync(request);
            await client.SendAsync();

            return success;
        }
    }
}

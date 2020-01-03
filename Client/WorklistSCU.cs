namespace SimpleDICOMToolkit.Client
{
    using Dicom;
    using Dicom.Network;
    using DicomClient = Dicom.Network.Client.DicomClient;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Logging;
    using Models;
    using System;

    public class WorklistSCU : IWorklistSCU
    {
        private readonly ILoggerService _logger = SimpleIoC.Get<ILoggerService>("filelogger");

        /// <summary>
        /// 参考
        /// https://github.com/fo-dicom/fo-dicom-samples/blob/master/Desktop/Worklist%20SCU/Program.cs
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="modality">Modality</param>
        /// <returns>Dataset</returns>
        public async Task<List<DicomDataset>> GetAllItemsFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null)
        {
            List<DicomDataset> worklistItems = new List<DicomDataset>();

            DicomCFindRequest worklistRequest = /*DicomCFindRequest.*/CreateWorklistQuery(null, null, localAET, null, modality,
                new DicomDateRange(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59)));

            worklistRequest.OnResponseReceived += (request, response) => 
            {
                if (!response.HasDataset)
                {
                    if (response.Status == DicomStatus.Success)
                        _logger.Debug("worklist response END.");
                    else
                        _logger.Debug("worklist response has [NO DATASET].");

                    return;
                }

                if (response.Status != DicomStatus.Success && 
                    response.Status != DicomStatus.Pending && 
                    response.Status != DicomStatus.QueryRetrieveOptionalKeysNotSupported)
                {
                    _logger.Error("worklist response error - [{0}]", response.Status);
                    return;
                }

                worklistItems.Add(response.Dataset);
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);

            await client.AddRequestAsync(worklistRequest);
            await client.SendAsync();

            return worklistItems;
        }

        public async Task<List<SimpleWorklistResult>> GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null)
        {
            List<SimpleWorklistResult> worklistResults = new List<SimpleWorklistResult>();

            DicomCFindRequest worklistRequest = /*DicomCFindRequest.*/CreateWorklistQuery(null, null, localAET, null, modality, 
                new DicomDateRange(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59)));  // 时间限制：当天 00:00:00 ~ 23:59:59

            worklistRequest.OnResponseReceived += (request, response) =>
            {
                if (!response.HasDataset)
                {
                    if (response.Status == DicomStatus.Success)
                        _logger.Debug("worklist response END.");
                    else
                        _logger.Debug("worklist response has [NO DATASET].");

                    return;
                }

                if (response.Status != DicomStatus.Success && 
                    response.Status != DicomStatus.Pending &&
                    response.Status != DicomStatus.QueryRetrieveOptionalKeysNotSupported)
                {
                    _logger.Error("worklist response error - [{0}]", response.Status);
                    return;
                }

                worklistResults.Add(GetWorklistResultFromDataset(response.Dataset));
            };

            DicomClient client = new DicomClient(serverIp, serverPort, false, localAET, serverAET);

            await client.AddRequestAsync(worklistRequest);
            await client.SendAsync();

            return worklistResults;
        }

        /// <summary>
        /// Convenience method for creating a C-FIND modality worklist query.
        /// from https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/Network/DicomCFindRequest.cs
        /// </summary>
        /// <param name="patientId">Patient ID.</param>
        /// <param name="patientName">Patient name.</param>
        /// <param name="stationAE">Scheduled station Application Entity Title.</param>
        /// <param name="stationName">Scheduled station name.</param>
        /// <param name="modality">Modality.</param>
        /// <param name="scheduledDateTime">Scheduled procedure step start time.</param>
        /// <returns>C-FIND modality worklist query object.</returns>
        public DicomCFindRequest CreateWorklistQuery(
            string patientId = null,
            string patientName = null,
            string stationAE = null,
            string stationName = null,
            string modality = null,
            DicomDateRange scheduledDateTime = null)
        {
            var dimse = new DicomCFindRequest(DicomUID.ModalityWorklistInformationModelFIND);
            dimse.Dataset.Add(DicomTag.PatientID, patientId);
            dimse.Dataset.Add(DicomTag.PatientName, patientName);
            /* yzh
             * 添加年龄tag
             **/
            dimse.Dataset.Add(DicomTag.PatientAge, string.Empty);
            // END
            dimse.Dataset.Add(DicomTag.IssuerOfPatientID, string.Empty);
            dimse.Dataset.Add(DicomTag.PatientSex, string.Empty);
            dimse.Dataset.Add(DicomTag.PatientWeight, string.Empty);
            dimse.Dataset.Add(DicomTag.PatientBirthDate, string.Empty);
            dimse.Dataset.Add(DicomTag.MedicalAlerts, string.Empty);
            dimse.Dataset.Add(DicomTag.PregnancyStatus, new ushort[0]);
            dimse.Dataset.Add(DicomTag.Allergies, string.Empty);
            dimse.Dataset.Add(DicomTag.PatientComments, string.Empty);
            dimse.Dataset.Add(DicomTag.SpecialNeeds, string.Empty);
            dimse.Dataset.Add(DicomTag.PatientState, string.Empty);
            dimse.Dataset.Add(DicomTag.CurrentPatientLocation, string.Empty);
            dimse.Dataset.Add(DicomTag.InstitutionName, string.Empty);
            dimse.Dataset.Add(DicomTag.AdmissionID, string.Empty);
            dimse.Dataset.Add(DicomTag.AccessionNumber, string.Empty);
            dimse.Dataset.Add(DicomTag.ReferringPhysicianName, string.Empty);
            dimse.Dataset.Add(DicomTag.AdmittingDiagnosesDescription, string.Empty);
            dimse.Dataset.Add(DicomTag.RequestingPhysician, string.Empty);
            dimse.Dataset.Add(DicomTag.StudyInstanceUID, string.Empty);
            dimse.Dataset.Add(DicomTag.StudyDescription, string.Empty);
            dimse.Dataset.Add(DicomTag.StudyID, string.Empty);
            dimse.Dataset.Add(DicomTag.ReasonForTheRequestedProcedure, string.Empty);
            dimse.Dataset.Add(DicomTag.StudyDate, string.Empty);
            dimse.Dataset.Add(DicomTag.StudyTime, string.Empty);

            dimse.Dataset.Add(DicomTag.RequestedProcedureID, string.Empty);
            dimse.Dataset.Add(DicomTag.RequestedProcedureDescription, string.Empty);
            dimse.Dataset.Add(DicomTag.RequestedProcedurePriority, string.Empty);
            dimse.Dataset.Add(new DicomSequence(DicomTag.RequestedProcedureCodeSequence));
            dimse.Dataset.Add(new DicomSequence(DicomTag.ReferencedStudySequence));

            dimse.Dataset.Add(new DicomSequence(DicomTag.ProcedureCodeSequence));

            var sps = new DicomDataset
            {
                { DicomTag.ScheduledStationAETitle, stationAE },
                { DicomTag.ScheduledStationName, stationName },
                { DicomTag.ScheduledProcedureStepStartDate, scheduledDateTime },
                { DicomTag.ScheduledProcedureStepStartTime, scheduledDateTime },
                { DicomTag.Modality, modality },
                { DicomTag.ScheduledPerformingPhysicianName, string.Empty },
                { DicomTag.ScheduledProcedureStepDescription, string.Empty },
                new DicomSequence(DicomTag.ScheduledProtocolCodeSequence),
                { DicomTag.ScheduledProcedureStepLocation, string.Empty },
                { DicomTag.ScheduledProcedureStepID, string.Empty },
                { DicomTag.RequestedContrastAgent, string.Empty },
                { DicomTag.PreMedication, string.Empty },
                { DicomTag.AnatomicalOrientationType, string.Empty }
            };
            dimse.Dataset.Add(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, sps));

            return dimse;
        }

        private SimpleWorklistResult GetWorklistResultFromDataset(DicomDataset dataset)
        {
            string name = dataset.GetString(DicomTag.PatientName);
            string sex = dataset.GetString(DicomTag.PatientSex);
            dataset.TryGetString(DicomTag.PatientAge, out string age);  // 请求了，但不一定存在
            string patId = dataset.GetString(DicomTag.PatientID);

            return new SimpleWorklistResult(name, sex, age, patId);
        }
    }
}

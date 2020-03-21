namespace SimpleDICOMToolkit.Client
{
    using Dicom;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueryRetrieveSCU
    {
        Task<List<DicomDataset>> QueryPatients(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null);

        Task<List<string>> QueryStudiesByPatientAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null, DicomDateRange studyDateTime = null);

        Task<List<string>> QuerySeriesByStudyAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string modality = null);

        Task<List<string>> QueryImagesByStudyAndSeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string modality = null);

        Task<List<DicomDataset>> GetImagesBySeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid);

        Task<bool?> MoveImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, string destAET, string studyInstanceUid, string seriesInstanceUid);
    }
}

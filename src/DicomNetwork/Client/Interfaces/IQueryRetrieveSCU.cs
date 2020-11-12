namespace SimpleDICOMToolkit.Client
{
#if FellowOakDicom5
    using FellowOakDicom;
#else
    using Dicom;
#endif
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IQueryRetrieveSCU
    {
        /// <summary>
        /// Query patients info from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="patientId">patient ID</param>
        /// <param name="patientName">patient name</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>datasets</returns>
        ValueTask<IEnumerable<DicomDataset>> QueryPatientsAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Query studies from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="patientId">patient ID</param>
        /// <param name="patientName">patient name</param>
        /// <param name="studyDateTime">study date time range</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>datasets</returns>
        ValueTask<IEnumerable<DicomDataset>> QueryStudiesByPatientAsync(string serverIp, int serverPort, string serverAET, string localAET, string patientId = null, string patientName = null, DicomDateRange studyDateTime = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// query series from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="modality">modality</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>datasets</returns>
        ValueTask<IEnumerable<DicomDataset>> QuerySeriesByStudyAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string modality = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// query images from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="modality">modality</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>datasets</returns>
        ValueTask<IEnumerable<DicomDataset>> QueryImagesByStudyAndSeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string modality = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// get images from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>images dataset</returns>
        ValueTask<IEnumerable<DicomDataset>> GetImagesBySeriesAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, CancellationToken cancellationToken = default);

        /// <summary>
        /// get image from remote server
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="sopInstanceUid">SOP instance UID</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>image dataset</returns>
        ValueTask<DicomDataset> GetImagesBySOPInstanceAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, CancellationToken cancellationToken = default);

        /// <summary>
        /// send C-MOVE request
        /// </summary>
        /// <param name="serverIp">server IP</param>
        /// <param name="serverPort">server port</param>
        /// <param name="serverAET">server AET</param>
        /// <param name="localAET">local AET</param>
        /// <param name="destAET">destination AET, need registered in server</param>
        /// <param name="studyInstanceUid">study instance UID</param>
        /// <param name="seriesInstanceUid">series instance UID</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>true if success, else false</returns>
        ValueTask<bool?> MoveImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, string destAET, string studyInstanceUid, string seriesInstanceUid, CancellationToken cancellationToken = default);
    }
}

namespace SimpleDICOMToolkit.Client
{
    using Dicom;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Models;

    public interface IWorklistSCU
    {
        /// <summary>
        /// Get Worklist Items Dataset
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="modality">Modality</param>
        /// <returns>Dataset</returns>
        ValueTask<List<DicomDataset>> GetAllItemsFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null);

        /// <summary>
        /// Get Simple Worklist Items Info
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="modality">Modality</param>
        /// <param name="fallbackEncoding">Encoding to use if encoding cannot be obtained from dataset</param>
        /// <returns>Simple result</returns>
        ValueTask<List<SimpleWorklistResult>> GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null, Encoding fallbackEncoding = null);

        ValueTask<(DicomUID affectedInstanceUid, string studyInstanceUid, bool result)> SendMppsInProgressAsync(string serverIp, int serverPort, string serverAET, string localAET, DicomDataset worklistItem);

        ValueTask<bool> SendMppsCompletedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem);

        ValueTask<bool> SendMppsDiscontinuedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem);
    }
}

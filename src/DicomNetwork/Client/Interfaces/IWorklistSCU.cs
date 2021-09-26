namespace SimpleDICOMToolkit.Client
{
    using FellowOakDicom;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;

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
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Dataset</returns>
        ValueTask<IEnumerable<DicomDataset>> GetAllItemsFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Simple Worklist Items Info
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="worklistResults">worklist results</param>
        /// <param name="modality">Modality</param>
        /// <param name="fallbackEncoding">Encoding to use if encoding cannot be obtained from dataset</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        ValueTask GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, ICollection<SimpleWorklistResult> worklistResults, string modality = null, Encoding fallbackEncoding = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Simple Worklist Items Info
        /// </summary>
        /// <param name="serverIp">Remote IP</param>
        /// <param name="serverPort">Remote Port</param>
        /// <param name="serverAET">Remote AET</param>
        /// <param name="localAET">Local AET</param>
        /// <param name="modality">Modality</param>
        /// <param name="fallbackEncoding">Encoding to use if encoding cannot be obtained from dataset</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Simple result</returns>
        ValueTask<IEnumerable<SimpleWorklistResult>> GetAllResultFromWorklistAsync(string serverIp, int serverPort, string serverAET, string localAET, string modality = null, Encoding fallbackEncoding = null, CancellationToken cancellationToken = default);

        ValueTask<(DicomUID affectedInstanceUid, string studyInstanceUid, bool result)> SendMppsInProgressAsync(string serverIp, int serverPort, string serverAET, string localAET, DicomDataset worklistItem);

        ValueTask<bool> SendMppsCompletedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem);

        ValueTask<bool> SendMppsDiscontinuedAsync(string serverIp, int serverPort, string serverAET, string localAET, string studyInstanceUid, DicomUID affectedInstanceUid, DicomDataset worklistItem);
    }
}

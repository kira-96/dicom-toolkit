namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public interface IPrintSCU
    {
        /// <summary>
        /// print films
        /// </summary>
        /// <param name="serverIp">Server IP Addr</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="serverAET">Server AE Title</param>
        /// <param name="localAET">Client AE Title</param>
        /// <param name="options">Print Options</param>
        /// <param name="images">images for print</param>
        /// <returns>Task</returns>
        Task PrintImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, PrintOptions options, IEnumerable<Bitmap> images);
    }
}

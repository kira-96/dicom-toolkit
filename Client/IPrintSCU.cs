namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public interface IPrintSCU
    {
        Task PrintImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<Bitmap> images);
    }
}

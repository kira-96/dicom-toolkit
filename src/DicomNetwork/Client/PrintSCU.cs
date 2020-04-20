namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public class PrintSCU : IPrintSCU
    {
        public async Task PrintImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, PrintOptions options, IEnumerable<Bitmap> images)
        {
            PrintJob printJob = new PrintJob(options)
            {
                RemoteAddress = serverIp,
                RemotePort = serverPort,
                CallingAE = localAET,
                CalledAE = serverAET
            };

            foreach (Bitmap image in images)
            {
                printJob.StartFilmBox(options);
                printJob.AddImage(image, 0);
                printJob.EndFilmBox();
            }

            await printJob.Print();
        }
    }
}

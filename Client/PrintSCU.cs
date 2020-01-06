namespace SimpleDICOMToolkit.Client
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public class PrintSCU : IPrintSCU
    {
        public async Task PrintImagesAsync(string serverIp, int serverPort, string serverAET, string localAET, IEnumerable<Bitmap> images)
        {
            PrintJob printJob = new PrintJob("DICOM PRINT JOB", "BLUE FILM")
            {
                RemoteAddress = serverIp,
                RemotePort = serverPort,
                CallingAE = localAET,
                CalledAE = serverAET
            };

            printJob.FilmSession.IsColor = false;

            foreach (Bitmap image in images)
            {
                printJob.StartFilmBox("STANDARD\\1,1", "PORTRAIT", "14INX17IN");
                printJob.AddImage(image, 0);
                printJob.EndFilmBox();
            }

            await printJob.Print();
        }
    }
}

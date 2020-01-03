namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Dicom.Imaging;
    using Stylet;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Utils;

    public class PreviewImageViewModel : Screen
    {
        private BitmapImage _imageSource;

        public BitmapImage ImageSource
        {
            get => _imageSource;
            private set => SetAndNotify(ref _imageSource, value);
        }

        public PreviewImageViewModel(string dcmfilepath)
        {
            ShowImage(dcmfilepath).Wait();
        }

        private async Task ShowImage(string dcmfilepath)
        {
            if (!File.Exists(dcmfilepath))
                return;

            DicomFile dcmFile = await DicomFile.OpenAsync(dcmfilepath);
            DicomImage image = new DicomImage(dcmFile.Dataset);

            using (IImage iimage = image.RenderImage())
            {
                ImageSource = iimage.AsWriteableBitmap().AsBitmapImage();
            }

            // set window title
            DisplayName = dcmFile.Dataset.GetString(DicomTag.PatientName);
        }
    }
}

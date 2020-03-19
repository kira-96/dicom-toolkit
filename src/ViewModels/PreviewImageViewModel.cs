namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Dicom.Imaging;
    using Stylet;
    using System.IO;
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
            if (!File.Exists(dcmfilepath))
                return;

            DicomFile dcmFile = DicomFile.Open(dcmfilepath);

            ShowImage(dcmFile.Dataset);
        }

        public PreviewImageViewModel(DicomDataset dataset)
        {
            ShowImage(dataset);
        }

        private void ShowImage(DicomDataset dataset)
        {
            DicomImage image = new DicomImage(dataset);

            using (IImage iimage = image.RenderImage())
            {
                ImageSource = iimage.AsWriteableBitmap().AsBitmapImage();
            }

            // set window title
            DisplayName = dataset.GetString(DicomTag.PatientName);
        }
    }
}

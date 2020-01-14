namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom.Imaging;
    using Stylet;
    using StyletIoC;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Client;
    using Services;
    using Models;
    using Utils;

    public class PrintPreviewViewModel : Screen, IHandle<PrintRequestItem>
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IDialogServiceEx _dialogService;

        [Inject]
        private IPrintSCU _printSCU;

        private string _dcmFile;

        public string DcmFile
        {
            get => _dcmFile;
            private set => SetAndNotify(ref _dcmFile, value);
        }

        private BitmapImage _imageSource;

        public BitmapImage ImageSource
        {
            get => _imageSource;
            private set => SetAndNotify(ref _imageSource, value);
        }

        public PrintPreviewViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            ShowDcmImage(System.Environment.CurrentDirectory + "\\942A.dcm");
        }

        public void OpenImage()
        {
            _dialogService.ShowOpenFileDialog("DICOM Image (*.dcm;*.dic)|*.dcm;*.dic", false, null, OpenDcmImage);
        }

        public async void Handle(PrintRequestItem message)
        {
            if (ImageSource == null)
                return;

            _eventAggregator.Publish(new BusyStateItem(true), nameof(PrintPreviewViewModel));

            await _printSCU.PrintImagesAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, 
                new List<Bitmap>() { ImageSource.AsBitmap() });

            _eventAggregator.Publish(new BusyStateItem(false), nameof(PrintPreviewViewModel));
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            base.OnClose();
        }

        private void OpenDcmImage(bool? result, string[] files)
        {
            if (result != true)
                return;

            ShowDcmImage(files[0]);
        }

        private void ShowDcmImage(string file)
        {
            if (!File.Exists(file))
                return;

            DicomImage image = new DicomImage(file);

            using (IImage iimage = image.RenderImage())
            {
                ImageSource = iimage.AsWriteableBitmap().AsBitmapImage();
            }

            // save file path
            DcmFile = file;
        }
    }
}

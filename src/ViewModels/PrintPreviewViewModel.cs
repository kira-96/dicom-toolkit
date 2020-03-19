namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom.Imaging;
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Media.Imaging;
    using Client;
    using Services;
    using Models;
    using Utils;
    using System.Windows;

    public class PrintPreviewViewModel : Screen, IHandle<PrintRequestItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IDialogServiceEx _dialogService;

        [Inject]
        private IPrintSCU _printSCU;

        private List<WriteableBitmap> _images = new List<WriteableBitmap>();

        private int _currentIndex = 0;

        public int CurrentIndex
        {
            get => _currentIndex;
            private set
            {
                SetAndNotify(ref _currentIndex, value);
                NotifyOfPropertyChange(() => ImageSource);
                NotifyOfPropertyChange(() => CanShowPrev);
                NotifyOfPropertyChange(() => CanShowNext);
            }
        }

        public WriteableBitmap ImageSource
        {
            get
            {
                if (_currentIndex < _images.Count)
                {
                    return _images[_currentIndex];
                }

                return null;
            }
        }

        public PrintPreviewViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            AddDcmImage(System.Environment.CurrentDirectory + "\\942A.dcm");
        }

        public async void Handle(PrintRequestItem message)
        {
            if (ImageSource == null)
                return;

            _eventAggregator.Publish(new BusyStateItem(true), nameof(PrintPreviewViewModel));

            PrintOptions options = new PrintOptions()
            {
                Orientation = FilmOrientation.Portrail,
                FilmSize = FilmSize.E14InX17In,
                MagnificationType = MagnificationType.None,
                MediumType = MediumType.BlueFilm
            };

            List<Bitmap> images = new List<Bitmap>();
            foreach (var image in _images)
            {
                images.Add(image.AsBitmap());
            }

            await _printSCU.PrintImagesAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, options, images);

            _eventAggregator.Publish(new BusyStateItem(false), nameof(PrintPreviewViewModel));
        }

        public void OnDragFilesOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        public void OnDropFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            System.Array files = e.Data.GetData(DataFormats.FileDrop) as System.Array;

            foreach (object file in files)
            {
                if (!File.Exists(file.ToString()))
                {
                    continue;
                }

                AddDcmImage(file.ToString());
            }
        }

        private void AddDcmImage(string file)
        {
            if (!File.Exists(file))
                return;

            DicomImage image = new DicomImage(file);

            using (IImage iimage = image.RenderImage())
            {
                _images.Add(iimage.AsWriteableBitmap());
            }

            // update Display
            CurrentIndex = _images.Count - 1;
        }

        public void RemoveCurrentImage()
        {
            _images.RemoveAt(_currentIndex);

            CurrentIndex = _currentIndex;
        }

        public bool CanShowPrev => CurrentIndex > 0;
        public bool CanShowNext => CurrentIndex < _images.Count - 1;

        public void ShowPrev()
        {
            if (_currentIndex > 0)
            {
                CurrentIndex -= 1;
            }
        }

        public void ShowNext()
        {
            if (_currentIndex < _images.Count - 1)
            {
                CurrentIndex += 1;
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

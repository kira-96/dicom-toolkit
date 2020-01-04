namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using StyletIoC;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Logging;
    using Models;
    using Services;

    public class DcmItemsViewModel : Screen, IHandle<UpdateDicomElementItem>
    {
        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IDialogServiceEx _dialogService;

        private IEventAggregator _eventAggregator;

        private DicomDataset _currentDataset;

        public BindableCollection<DcmItem> DicomItems { get; private set; } = new BindableCollection<DcmItem>();

        private DcmItem _currentItem;

        public DcmItemsViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "DICOM Dump";
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            base.OnClose();
        }

        public void Handle(UpdateDicomElementItem message)
        {
            AddOrUpdateDicomItem(message.Dataset, message.VR, message.Tag, message.Values);
        }

        public void OnDragFileOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        public async void OnDropFile(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            if (!File.Exists(path))
                return;

            string ext = Path.GetExtension(path).ToLower();

            if (ext == ".dcm" ||
                ext == ".dic")
            {
                await OpenDcmFile(path);
            }
        }

        private async Task OpenDcmFile(string file)
        {
            DicomFile dcmFile = await DicomFile.OpenAsync(file);

            _currentDataset = dcmFile.Dataset;

            var enumerator = _currentDataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                DicomItems.Add(new DcmItem(enumerator.Current));
            }
        }

        public bool IsPixelDataItem(DcmItem item)
        {
            return new DicomTag(item.Group, item.Element).CompareTo(DicomTag.PixelData) == 0;
        }

        public void ShowDcmImage()
        {
            _windowManager.ShowDialog(new PreviewImageViewModel(_currentDataset));
        }

        public void EditDicomItem(DcmItem item)
        {
            _currentItem = item;
            _windowManager.ShowDialog(new EditDicomItemViewModel(GetItemDataset(item), new DicomTag(item.Group, item.Element)));
        }

        private DicomDataset GetItemDataset(DcmItem item)
        {
            if (DicomItems.Contains(item))
                return _currentDataset;

            foreach (DcmItem seq in DicomItems.Where(i => i.TagType == DcmTagType.Sequence))
            {
                for (int i = 0; i < seq.SequenceItems.Count; i++)
                {
                    if (seq.SequenceItems[i].SequenceItems.Contains(item))
                    {
                        return _currentDataset.GetDicomItem<DicomSequence>(new DicomTag(seq.Group, seq.Element)).Items[i];
                    }
                }
            }

            return null;
        }

        public void SaveNewDicom()
        {
            _dialogService.ShowSaveFileDialog("DICOM Image (*.dcm;*.dic)|*.dcm;*.dic", null,
                async (result, path) =>
                {
                    if (result == true)
                    {
                        await new DicomFile(_currentDataset).SaveAsync(path);
                    }
                });
        }

        private void AddOrUpdateDicomItem(DicomDataset dataset, DicomVR vr, DicomTag tag, string[] values)
        {
            try
            {
                dataset.AddOrUpdate(vr, tag, values);
            }
            catch (System.Exception e)
            {
                _logger.Error(e);
                return;
            }

            _currentItem.UpdateItem(dataset.GetDicomItem<DicomElement>(tag));
        }
    }
}

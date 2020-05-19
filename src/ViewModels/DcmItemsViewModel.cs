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
#pragma warning disable IDE0044, 0649
        [Inject(Key = "filelogger")]
        private ILoggerService _logger;

        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IDialogServiceEx _dialogService;
#pragma warning disable IDE0044, 0649

        private readonly IEventAggregator _eventAggregator;

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

            if (DicomFile.HasValidHeader(path))
            {
                await OpenDcmFile(path);
            }
        }

        private async Task OpenDcmFile(string file)
        {
            DicomItems.Clear();

            DicomFile dcmFile = await DicomFile.OpenAsync(file);
#pragma warning disable CS0618 // 类型或成员已过时
            dcmFile.Dataset.AutoValidate = false;
#pragma warning restore CS0618 // 类型或成员已过时

            _currentDataset = dcmFile.Dataset;

            var enumerator = _currentDataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                DicomItems.Add(new DcmItem(enumerator.Current));
            }
        }

        public void DcmItemTapped(DcmItem item)
        {
            if (item.TagType != DcmTagType.Tag)
                return;

            if (IsPixelDataItem(item))
            {
                ShowDcmImage(item);
            }
            else
            {
                EditDicomItem(item);
            }
        }

        public bool IsPixelDataItem(DcmItem item)
        {
            return item.DcmTag.CompareTo(DicomTag.PixelData) == 0;
        }

        public void ShowDcmImage(DcmItem item)
        {
            _currentItem = item;
            _windowManager.ShowDialog(new PreviewImageViewModel(GetItemDataset(item)));
        }

        public void EditDicomItem(DcmItem item)
        {
            _currentItem = item;
            _windowManager.ShowDialog(new EditDicomItemViewModel(GetItemDataset(item), item.DcmTag));
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
                        return _currentDataset.GetDicomItem<DicomSequence>(seq.DcmTag).Items[i];
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
            string charset = _currentDataset.GetSingleValueOrDefault(DicomTag.SpecificCharacterSet, "ISO_IR 192");

            try
            {
                dataset.AddOrUpdate(vr, tag, DicomEncoding.GetEncoding(charset), values);
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

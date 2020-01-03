namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using StyletIoC;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using Models;

    public class DcmItemsViewModel : Screen
    {
        [Inject]
        private IWindowManager _windowManager;

        private string _currentFile;

        public BindableCollection<DcmItem> DicomItems { get; private set; } = new BindableCollection<DcmItem>();

        public DcmItemsViewModel()
        {
            DisplayName = "DICOM Dump";

            // OpenDcmFile(System.Environment.CurrentDirectory + "\\sample.dcm").Wait();
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

            var enumerator = dcmFile.Dataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                DicomItems.Add(new DcmItem(enumerator.Current));
            }

            this._currentFile = file;
        }

        public bool IsPixelDataItem(DcmItem item)
        {
            return item.TagDescription == DicomTag.PixelData.DictionaryEntry.Name;
        }

        public void ShowDcmImage()
        {
            _windowManager.ShowDialog(new PreviewImageViewModel(_currentFile));
        }
    }
}

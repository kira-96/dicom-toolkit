namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using System.Windows;
    using Client;
    using Services;
    using Models;

    public class CStoreFileListViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private IDialogServiceEx _dialogService;

        [Inject]
        private ICStoreSCU _cstoreSCU;

        public BindableCollection<CStoreItem> FileList { get; private set; } = new BindableCollection<CStoreItem>();

        public CStoreFileListViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(CStoreFileListViewModel));
        }

        public async void Handle(ClientMessageItem message)
        {
            if (FileList.Count == 0)
                return;

            _eventAggregator.Publish(new BusyStateItem(true), nameof(CStoreFileListViewModel));

            await _cstoreSCU.StoreImageAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, FileList);

            _eventAggregator.Publish(new BusyStateItem(false), nameof(CStoreFileListViewModel));
        }

        public void AddFiles()
        {
            _dialogService.ShowOpenFileDialog("DICOM Image (*.dcm;*.dic)|*.dcm;*.dic", true, null, AddDcmFilesToList);
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
                string path = file as string;

                if (Directory.Exists(path))  // 文件夹
                {
                    DirectoryInfo dir = new DirectoryInfo(path);

                    FileInfo[] fileinfos = dir.GetFiles();

                    foreach (FileInfo info in fileinfos)
                    {
                        string ext = info.Extension;

                        if (ext.ToLower() == ".dcm" ||
                            ext.ToLower() == ".dic")
                        {
                            if (!info.Exists)
                                continue;

                            FileList.Add(new CStoreItem(FileList.Count, info.FullName));
                        }
                    }

                    continue;
                }

                if (path.ToLower().EndsWith(".dcm") ||
                    path.ToLower().EndsWith(".dic"))
                {
                    if (!File.Exists(path))
                        continue;

                    FileList.Add(new CStoreItem(FileList.Count, path));
                }
            }
        }

        public void PreviewCStoreItem(CStoreItem item)
        {
            _windowManager.ShowDialog(new PreviewImageViewModel(item.File));
        }

        public void DeleteCStoreItem(CStoreItem item)
        {
            FileList.Remove(item);
            ReIndexItems();
        }

        public void ClearItems()
        {
            FileList.Clear();
        }

        private void AddDcmFilesToList(bool? result, string[] files)
        {
            foreach (string file in files)
            {
                FileList.Add(new CStoreItem(FileList.Count, file));
            }
        }

        private void ReIndexItems()
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                FileList[i].Id = i;
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

namespace SimpleDICOMToolkit.ViewModels
{
#if FellowOakDicom5
    using FellowOakDicom;
#else
    using Dicom;
#endif
    using Stylet;
    using StyletIoC;
    using System;
    using System.IO;
    using System.Windows;
    using Client;
    using Infrastructure;
    using Models;
    using Services;

    public class StoreFileListViewModel : Screen, IHandle<DicomRequestEvent>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private II18nService _i18NService;

        [Inject]
        private INotificationService _notificationService;

        [Inject]
        private IViewModelFactory _viewModelFactory;

        [Inject]
        private IDialogServiceEx _dialogService;

        [Inject]
        private IStoreSCU _storeSCU;

        public BindableCollection<IStoreItem> FileList { get; private set; } = new BindableCollection<IStoreItem>();

        public StoreFileListViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(StoreFileListViewModel));
        }

        public async void Handle(DicomRequestEvent message)
        {
            if (FileList.Count == 0)
                return;

            _eventAggregator.Publish(new BusyStateEvent(true), nameof(StoreFileListViewModel));

            foreach (var file in FileList)
            {
                file.Status = StoreItemStatus.Waiting;
            }

            try
            {
                int errors = await _storeSCU.StoreImageAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, FileList);

                string content = string.Format(
                                    _i18NService.GetXmlStringByKey("ToastStoreComplete"),
                                    FileList.Count - errors, errors);
                // 这里不使用 await，否则当前线程会阻塞直到toast显示完成
                _ = _notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3));
            }
            finally
            {
                _eventAggregator.Publish(new BusyStateEvent(false), nameof(StoreFileListViewModel));
            }
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

            Array files = e.Data.GetData(DataFormats.FileDrop) as Array;

            foreach (object file in files)
            {
                string path = file as string;

                if (Directory.Exists(path))  // 文件夹
                {
                    DirectoryInfo dir = new DirectoryInfo(path);

                    FileInfo[] fileinfos = dir.GetFiles();

                    foreach (FileInfo info in fileinfos)
                    {
                        if (!info.Exists)
                            continue;

                        if (!DicomFile.HasValidHeader(info.FullName))
                            continue;

                        FileList.Add(new StoreItem(FileList.Count, info.FullName));
                    }

                    continue;
                }

                if (File.Exists(path) && DicomFile.HasValidHeader(path))
                {
                    FileList.Add(new StoreItem(FileList.Count, path));
                }
            }
        }

        public void PreviewCStoreItem(IStoreItem item)
        {
            var preview = _viewModelFactory.GetPreviewImageViewModel();
            preview.Initialize(item.File);

            _windowManager.ShowDialog(preview, this);
        }

        public void DeleteCStoreItem(IStoreItem item)
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
                FileList.Add(new StoreItem(FileList.Count, file));
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

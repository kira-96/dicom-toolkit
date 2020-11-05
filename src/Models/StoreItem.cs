namespace SimpleDICOMToolkit.Models
{
    using Stylet;
    using System.IO;
    using Infrastructure;

    /// <summary>
    /// 列表显示的待发送文件
    /// </summary>
    public class StoreItem : PropertyChangedBase, IStoreItem
    {
        private int _id;
        public int Id 
        { 
            get => _id; 
            set => SetAndNotify(ref _id, value);
        }

        public string File { get; private set; }

        public string FileName { get; private set; }

        private StoreItemStatus _status = StoreItemStatus.Waiting;

        public StoreItemStatus Status
        {
            get => _status;
            set => SetAndNotify(ref _status, value);
        }

        public StoreItem(int id, string file)
        {
            _id = id;
            File = file;
            FileName = Path.GetFileName(file);
        }
    }
}

namespace SimpleDICOMToolkit.Models
{
    using Stylet;
    using System.IO;

    public enum CStoreItemStatus
    {
        Waiting,
        Success,
        Failed
    }

    public class CStoreItem : PropertyChangedBase
    {
        private int _id;
        public int Id 
        { 
            get => _id; 
            set => SetAndNotify(ref _id, value);
        }

        public string File { get; private set; }

        public string FileName { get; private set; }

        private CStoreItemStatus _status = CStoreItemStatus.Waiting;

        public CStoreItemStatus Status
        {
            get => _status;
            set => SetAndNotify(ref _status, value);
        }

        public CStoreItem(int id, string file)
        {
            _id = id;
            File = file;
            FileName = Path.GetFileName(file);
        }
    }
}

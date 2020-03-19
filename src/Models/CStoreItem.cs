namespace SimpleDICOMToolkit.Models
{
    using Stylet;
    using System.IO;

    /// <summary>
    /// 文件发送状态
    /// </summary>
    public enum CStoreItemStatus
    {
        Waiting,  // 等待
        Success,  // 成功
        Failed    // 失败
    }

    /// <summary>
    /// 列表显示的待发送文件
    /// </summary>
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

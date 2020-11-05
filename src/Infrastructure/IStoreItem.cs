namespace SimpleDICOMToolkit.Infrastructure
{
    /// <summary>
    /// 文件发送状态
    /// </summary>
    public enum StoreItemStatus
    {
        Waiting,  // 等待
        Success,  // 成功
        Failed    // 失败
    }

    public interface IStoreItem
    {
        public int Id { get; set; }

        public string File { get; }

        public string FileName { get; }

        public StoreItemStatus Status { get; set; }
    }
}

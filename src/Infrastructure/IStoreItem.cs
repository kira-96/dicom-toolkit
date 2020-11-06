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
        int Id { get; set; }

        string File { get; }

        string FileName { get; }

        StoreItemStatus Status { get; set; }
    }
}

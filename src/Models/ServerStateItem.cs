namespace SimpleDICOMToolkit.Models
{
    public class ServerStateItem
    {
        public bool IsRuning { get; private set; }

        public ServerStateItem(bool isRunning)
        {
            IsRuning = isRunning;
        }
    }
}

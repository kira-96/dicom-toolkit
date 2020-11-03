namespace SimpleDICOMToolkit.Models
{
    public class ServerStateEvent
    {
        public bool IsRuning { get; private set; }

        public ServerStateEvent(bool isRunning)
        {
            IsRuning = isRunning;
        }
    }
}

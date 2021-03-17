namespace SimpleDICOMToolkit.Models
{
    public class ServerStateEvent
    {
        public bool IsRuning { get; private set; }

        public ServerStateEvent(bool isRunning)
        {
            IsRuning = isRunning;
        }

        public override string ToString()
        {
            return string.Format("Server is running: {0}", IsRuning);
        }
    }
}

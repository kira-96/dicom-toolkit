namespace SimpleDICOMToolkit.Models
{
    public class BusyStateEvent
    {
        public bool IsBusy { get; private set; }

        public BusyStateEvent(bool isBusy)
        {
            IsBusy = isBusy;
        }
    }
}

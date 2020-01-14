namespace SimpleDICOMToolkit.Models
{
    public class BusyStateItem
    {
        public bool IsBusy { get; private set; }

        public BusyStateItem(bool isBusy)
        {
            IsBusy = isBusy;
        }
    }
}

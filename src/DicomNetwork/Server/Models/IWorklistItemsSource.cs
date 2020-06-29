namespace SimpleDICOMToolkit.Server
{
    using System.Collections.ObjectModel;
    using Models;

    public interface IWorklistItemsSource
    {
        ObservableCollection<WorklistItem> WorklistItems { get; }

        void AddItem(WorklistItem item);

        void RemoveItem(WorklistItem item);

        void RemoveAt(int index);
    }
}

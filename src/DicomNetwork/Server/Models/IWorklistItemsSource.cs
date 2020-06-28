using System.Collections.ObjectModel;

namespace SimpleDICOMToolkit.Server
{
    public interface IWorklistItemsSource
    {
        ObservableCollection<WorklistItem> WorklistItems { get; }

        void AddItem(WorklistItem item);

        void RemoveItem(WorklistItem item);

        void RemoveAt(int index);
    }
}

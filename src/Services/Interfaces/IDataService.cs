using System.Collections.Generic;
using SimpleDICOMToolkit.Models;

namespace SimpleDICOMToolkit.Services
{
    public interface IDataService
    {
        bool IsConnected { get; }

        void ConnectDatabase(string connectionString);

        void DisconnectDatabase();

        IEnumerable<WorklistItem> GetWorklistItems();

        void AddWorklistItem(WorklistItem worklistItem);

        void RemoveWorklistItem(WorklistItem worklistItem);

        void UpdateWorklistItemStatus(WorklistItem worklistItem);
    }
}

using System.Collections.Generic;
using SimpleDICOMToolkit.Models;

namespace SimpleDICOMToolkit.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Is connected to database
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connect to database
        /// </summary>
        /// <param name="connectionString">connection string</param>
        void ConnectDatabase(string connectionString);

        /// <summary>
        /// Disconnect database
        /// </summary>
        void DisconnectDatabase();

        /// <summary>
        /// Query all worklist items from database
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorklistItem> GetWorklistItems();

        /// <summary>
        /// Add new worklist item to database
        /// </summary>
        /// <param name="worklistItem"></param>
        void AddWorklistItem(WorklistItem worklistItem);

        /// <summary>
        /// Remove worklist item from database
        /// </summary>
        /// <param name="worklistItem"></param>
        void RemoveWorklistItem(WorklistItem worklistItem);

        /// <summary>
        /// Update worklist item
        /// </summary>
        /// <param name="worklistItem"></param>
        void UpdateWorklistItemStatus(WorklistItem worklistItem);
    }
}

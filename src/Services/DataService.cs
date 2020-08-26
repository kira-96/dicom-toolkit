using LiteDB;
using System.Collections.Generic;
using SimpleDICOMToolkit.Models;

namespace SimpleDICOMToolkit.Services
{
    public class DataService : IDataService
    {
        private LiteDatabase db;

        public bool IsConnected { get; private set; }

        public void ConnectDatabase(string connectionString)
        {
            if (IsConnected)
            {
                DisconnectDatabase();
            }

            db = new LiteDatabase(connectionString);

            IsConnected = true;
        }

        public void DisconnectDatabase()
        {
            if (IsConnected)
            {
                db.Dispose();
                IsConnected = false;
            }
        }

        public IEnumerable<WorklistItem> GetWorklistItems()
        {
            if (!IsConnected)
                return null;

            return db.GetCollection<WorklistItem>("worklistitems").Query().ToEnumerable();
        }

        public void AddWorklistItem(WorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Insert(worklistItem.ToBsonDocument());
        }

        public void RemoveWorklistItem(WorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Delete(worklistItem.Id);
        }

        public void UpdateWorklistItemStatus(WorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Update(worklistItem.ToBsonDocument());
        }
    }
}

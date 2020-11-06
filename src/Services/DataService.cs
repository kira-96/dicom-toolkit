using LiteDB;
using System.Collections.Generic;
using SimpleDICOMToolkit.Infrastructure;
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

        public IEnumerable<IWorklistItem> GetWorklistItems()
        {
            if (!IsConnected)
                return null;

            return db.GetCollection<WorklistItem>("worklistitems").Query().ToEnumerable();
        }

        public void AddWorklistItem(IWorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Insert(((WorklistItem)worklistItem).ToBsonDocument());
        }

        public void RemoveWorklistItem(IWorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Delete(((WorklistItem)worklistItem).Id);
        }

        public void UpdateWorklistItemStatus(IWorklistItem worklistItem)
        {
            if (!IsConnected)
                return;

            var col = db.GetCollection("worklistitems");

            col.Update(((WorklistItem)worklistItem).ToBsonDocument());
        }
    }
}

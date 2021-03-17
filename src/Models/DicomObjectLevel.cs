using Stylet;
using System.Collections.Generic;
using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Models
{
    public class DicomObjectLevel : IDicomObjectLevel
    {
        public string Text { get; private set; }

        public string UID { get; }

        public bool HasChildren => Children.Count > 0;

        public Level Level { get; }

        public IDicomObjectLevel Parent { get; }

        public ICollection<IDicomObjectLevel> Children { get; }

        public DicomObjectLevel(string text, string uid, Level level, IDicomObjectLevel parent)
        {
            Text = text;
            UID = uid;
            Level = level;
            Parent = parent;
            Children = new BindableCollection<IDicomObjectLevel>();
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} {2}", Level, Text, UID);
        }
    }
}

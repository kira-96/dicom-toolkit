using Stylet;

namespace SimpleDICOMToolkit.Models
{
    public enum Level
    {
        Patient,
        Study,
        Series,
        Image
    }

    public interface IDicomObjectLevel
    {
        string Text { get; }

        string UID { get; }

        bool HasChildren { get; }

        Level Level { get; }

        IDicomObjectLevel Parent { get; }

        BindableCollection<IDicomObjectLevel> Children { get; }
    }

    public class DicomObjectLevel : IDicomObjectLevel
    {
        public string Text { get; private set; }

        public string UID { get; }

        public bool HasChildren => Children.Count > 0;

        public Level Level { get; }

        public IDicomObjectLevel Parent { get; }

        public BindableCollection<IDicomObjectLevel> Children { get; }

        public DicomObjectLevel(string text, string uid, Level level, IDicomObjectLevel parent)
        {
            Text = text;
            UID = uid;
            Level = level;
            Parent = parent;
            Children = new BindableCollection<IDicomObjectLevel>();
        }
    }
}

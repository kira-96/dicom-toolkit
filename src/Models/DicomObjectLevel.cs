using System.Collections.Generic;

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
        string DisplayName { get; }

        Level Level { get; }

        IDicomObjectLevel Parent { get; }

        List<IDicomObjectLevel> Children { get; }
    }

    public class DicomObjectLevel : IDicomObjectLevel
    {
        public string DisplayName { get; private set; }

        public Level Level { get; }

        public IDicomObjectLevel Parent { get; }

        public List<IDicomObjectLevel> Children { get; }

        public DicomObjectLevel(string name, Level level, IDicomObjectLevel parent)
        {
            DisplayName = name;
            Level = level;
            Parent = parent;
            Children = new List<IDicomObjectLevel>();
        }
    }
}

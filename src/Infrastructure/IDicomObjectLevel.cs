using System.Collections.Generic;

namespace SimpleDICOMToolkit.Infrastructure
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

        ICollection<IDicomObjectLevel> Children { get; }
    }
}

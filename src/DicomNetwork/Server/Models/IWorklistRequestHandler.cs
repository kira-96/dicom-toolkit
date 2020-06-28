using Dicom;
using System.Collections.Generic;

namespace SimpleDICOMToolkit.Server
{
    public interface IWorklistRequestHandler
    {
        IEnumerable<DicomDataset> FilterWorklistItems(DicomDataset request);
    }
}

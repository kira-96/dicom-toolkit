using FellowOakDicom;

namespace SimpleDICOMToolkit.Models
{
    /// <summary>
    /// 添加 DicomElement
    /// </summary>
    public class AddDicomElementEvent
    {
        public DicomDataset Dataset { get; }

        public DicomVR VR { get; }

        public string Tag { get; }

        public string[] Values { get; }

        public AddDicomElementEvent(DicomDataset dataset, DicomVR dicomVR, string tag, string[] values)
        {
            Dataset = dataset;
            VR = dicomVR;
            Tag = tag;
            Values = values;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Tag, VR);
        }
    }
}

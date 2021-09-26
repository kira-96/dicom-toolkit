using FellowOakDicom;

namespace SimpleDICOMToolkit.Models
{
    /// <summary>
    /// 更新修改后的DicomElement
    /// </summary>
    public class UpdateDicomElementEvent
    {
        public DicomDataset Dataset { get; }

        public DicomVR VR { get; }

        public DicomTag Tag { get; }

        public string[] Values { get; }

        public UpdateDicomElementEvent(DicomDataset dataset, DicomVR dicomVR, DicomTag tag, string[] values)
        {
            Dataset = dataset;
            VR = dicomVR;
            Tag = tag;
            Values = values;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Tag, VR, Tag.DictionaryEntry.Name);
        }
    }
}

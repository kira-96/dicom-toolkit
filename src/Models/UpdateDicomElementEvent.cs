#if FellowOakDicom5
using FellowOakDicom;
#else
using Dicom;
#endif

namespace SimpleDICOMToolkit.Models
{
    /// <summary>
    /// 更新修改后的DicomElement
    /// </summary>
    public class UpdateDicomElementEvent
    {
        public DicomDataset Dataset { get; private set; }

        public DicomVR VR { get; private set; }

        public DicomTag Tag { get; private set; }

        public string[] Values { get; private set; }

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

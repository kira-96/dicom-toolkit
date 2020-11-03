namespace SimpleDICOMToolkit.Models
{
    using Dicom;

    /// <summary>
    /// 添加 DicomElement
    /// </summary>
    public class AddDicomElementEvent
    {
        public DicomDataset Dataset { get; private set; }

        public DicomVR VR { get; private set; }

        public string Tag { get; private set; }

        public string[] Values { get; private set; }

        public AddDicomElementEvent(DicomDataset dataset, DicomVR dicomVR, string tag, string[] values)
        {
            Dataset = dataset;
            VR = dicomVR;
            Tag = tag;
            Values = values;
        }
    }
}

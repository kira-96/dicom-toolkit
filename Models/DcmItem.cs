namespace SimpleDICOMToolkit.Models
{
    using Dicom;
    using Stylet;

    public enum DcmTagType
    {
        Tag,
        Sequence,
        SequenceItem,
    }

    public class DcmItem
    {
        public string DcmTagCode { get; private set; }

        public string DcmVRCode { get; private set; }

        public string TagDescription { get; private set; }

        public string TagValue { get; private set; }

        public DcmTagType TagType { get; private set; } = DcmTagType.Tag;

        public string Header
        {
            get
            {
                switch (TagType)
                {
                    case DcmTagType.Tag:
                        return string.Format("{0} {1} {2} = <{3}>", DcmTagCode, DcmVRCode, TagDescription, TagValue);
                    case DcmTagType.Sequence:
                        return string.Format("{0} {1} {2}", DcmTagCode, DcmVRCode, TagDescription);
                    case DcmTagType.SequenceItem:
                        return TagDescription;
                    default:
                        return "";
                }
            }
        }

        public BindableCollection<DcmItem> SequenceItems { get; private set; }

        public DcmItem(DicomItem item)
        {
            DcmTagCode = string.Format("({0:X4},{1:X4})", item.Tag.Group, item.Tag.Element);
            DcmVRCode = item.ValueRepresentation.Code;
            TagDescription = item.Tag.DictionaryEntry.Name;

            if (item is DicomSequence seq)
            {
                TagType = DcmTagType.Sequence;
                SequenceItems = new BindableCollection<DcmItem>();

                foreach (DicomDataset dataset in seq.Items)
                {
                    DcmItem seqItem = new DcmItem(dataset)
                    { TagDescription = $"Item #{SequenceItems.Count}" };

                    SequenceItems.Add(seqItem);
                }
            }
            else if (item is DicomElement element)
            {
                if (element.Tag == DicomTag.PixelData)
                {
                    TagValue = "[Binary Pixel Data]";
                    return;
                }

                for (int i = 0; i < element.Count; i++)
                {
                    TagValue += element.Get<string>(i) + '\\';
                }

                TagValue = TagValue?.TrimEnd('\\');
            }
        }

        private DcmItem(DicomDataset dataset)
        {
            TagType = DcmTagType.SequenceItem;

            SequenceItems = new BindableCollection<DcmItem>();

            var enumerator = dataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SequenceItems.Add(new DcmItem(enumerator.Current));
            }
        }
    }
}

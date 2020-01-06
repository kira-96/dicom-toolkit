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

    /**
     * 这一块逻辑可能比较乱
     * 主要是Sequence可能有多个Item，每个Item又可能有多个Tag
     * 相互嵌套间形成了树状结构
     * 所以在View层使用了TreeView而不是ListView
     * 
     * ├─ Tag (00xx,xxxx)
     * ├─ Tag (00xx,xxxx)
     * ├─ Sequence (00xx,xxxx)
     * │   ├─ Item #0
     * │   │   ├─ Tag (00xx,xxxx)
     * │   │   ├─ Tag (00xx,xxxx)
     * │   │   └─ Tag (00xx,xxxx)
     * │   └─ Item #1
     * │       ├─ Tag (00xx,xxxx)
     * │       ├─ Tag (00xx,xxxx)
     * │       └─ Tag (00xx,xxxx)
     * ├─ Tag (00xx,xxxx)
     * ├─ ...
     * └─ (7FE0,0010) Pixel Data = <[Binary Pixel Data]>
     */

    public class DcmItem : PropertyChangedBase
    {
        public DicomTag DcmTag { get; private set; }

        private string _dcmVRCode;

        public string DcmVRCode
        {
            get => _dcmVRCode;
            private set
            {
                SetAndNotify(ref _dcmVRCode, value);
                NotifyOfPropertyChange(() => Header);
            }
        }

        public string TagDescription { get; private set; }

        private string _tagValue;

        public string TagValue
        {
            get => _tagValue;
            private set
            {
                SetAndNotify(ref _tagValue, value);
                NotifyOfPropertyChange(() => Header);
            }
        }

        public DcmTagType TagType { get; private set; } = DcmTagType.Tag;

        public string Header
        {
            get
            {
                switch (TagType)
                {
                    case DcmTagType.Tag:
                        return string.Format("({0:X4},{1:X4}) {2} {3} = <{4}>", DcmTag.Group, DcmTag.Element, DcmVRCode, TagDescription, TagValue);
                    case DcmTagType.Sequence:
                        return string.Format("({0:X4},{1:X4}) {2} {3}", DcmTag.Group, DcmTag.Element, DcmVRCode, TagDescription);
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
            DcmTag = item.Tag;
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
                if (element.Tag.CompareTo(DicomTag.PixelData) == 0)
                {
                    TagValue = "[Binary Pixel Data]";
                    return;
                }

                TagValue = "";

                for (int i = 0; i < element.Count; i++)
                {
                    TagValue += element.Get<string>(i) + '\\';
                }

                TagValue = TagValue?.TrimEnd('\\');
            }
            else if (item is DicomFragmentSequence fragment)
            {
                if (fragment.Tag.CompareTo(DicomTag.PixelData) == 0)
                {
                    TagValue = "[Binary Pixel Data]";
                    return;
                }

                TagValue = "[Binary Data]";
            }
            else
            {
                // do nothing
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

        public void UpdateItem(DicomElement element)
        {
            DcmVRCode = element.ValueRepresentation.Code;

            TagValue = "";

            for (int i = 0; i < element.Count; i++)
            {
                TagValue += element.Get<string>(i) + '\\';
            }

            TagValue = TagValue?.TrimEnd('\\');
        }
    }
}

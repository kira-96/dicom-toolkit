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

    public class DcmItem : PropertyChangedBase
    {
        public string DcmTagCode { get; private set; }

        private string _dcmVRCode;
        public string DcmVRCode 
        {
            get => _dcmVRCode;
            set
            {
                SetAndNotify(ref _dcmVRCode, value);
                NotifyOfPropertyChange(() => Header);
            }
        }

        public string TagDescription { get; private set; }

        private string _tagValue = "";

        public string TagValue
        {
            get => _tagValue;
            set
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
            _dcmVRCode = item.ValueRepresentation.Code;
            TagDescription = item.Tag.DictionaryEntry.Name;

            if (item is DicomSequence seq)
            {
                TagType = DcmTagType.Sequence;
                SequenceItems = new BindableCollection<DcmItem>();

                foreach (DicomDataset dataset in seq.Items)
                {
                    DcmItem seqItem = new DcmItem(dataset)
                    { TagDescription = $"item #{SequenceItems.Count}" };

                    SequenceItems.Add(seqItem);
                }
            }
            else if (item is DicomElement element)
            {
                if (element.Tag == DicomTag.PixelData)
                {
                    _tagValue = "[Binary Pixel Data]";
                    return;
                }

                for (int i = 0; i < element.Count; i++)
                {
                    _tagValue += element.Get<string>(i) + '\\';
                }

                _tagValue = _tagValue.TrimEnd('\\');
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

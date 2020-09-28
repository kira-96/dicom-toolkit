namespace SimpleDICOMToolkit.Models
{
    using Dicom;
    using Stylet;
    using System;

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
        /// <summary>
        /// Parent dataset
        /// </summary>
        public DicomDataset Dataset { get; }

        /// <summary>
        /// Parent sequence
        /// </summary>
        public DicomSequence Sequence { get; }

        public DicomTag DcmTag { get; private set; }

        private string _dcmVRCode;

        public string DcmVRCode
        {
            get => _dcmVRCode;
            private set
            {
                if (SetAndNotify(ref _dcmVRCode, value))
                {
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        public string TagDescription { get; private set; }

        private string _tagValue;

        public string TagValue
        {
            get => _tagValue;
            private set
            {
                if (SetAndNotify(ref _tagValue, value))
                {
                    NotifyOfPropertyChange(() => FormattedContent);
                }
            }
        }

        private string _additionalInfo;

        public string AdditionalInfo
        {
            get => _additionalInfo;
            private set
            {
                SetAndNotify(ref _additionalInfo, value);
            }
        }

        public DcmTagType TagType { get; private set; } = DcmTagType.Tag;

        private bool _isValid = true;

        public bool IsValid
        {
            get => _isValid;
            private set => SetAndNotify(ref _isValid, value);
        }

        public string Header
        {
            get
            {
                return TagType switch
                {
                    DcmTagType.Tag => string.Format("({0:X4},{1:X4}) {2} {3} = ", DcmTag.Group, DcmTag.Element, DcmVRCode, TagDescription),
                    DcmTagType.Sequence => string.Format("({0:X4},{1:X4}) {2} {3}", DcmTag.Group, DcmTag.Element, DcmVRCode, TagDescription),
                    DcmTagType.SequenceItem => string.Format("({0:X4},{1:X4}) {2}", DcmTag.Group, DcmTag.Element, TagDescription),
                    _ => "",
                };
            }
        }

        public string FormattedContent => TagType == DcmTagType.Tag ? string.Format("<{0}>", TagValue) : string.Empty;

        public BindableCollection<DcmItem> SequenceItems { get; private set; }

        public DcmItem(DicomItem item, DicomDataset dataset)
        {
            Dataset = dataset;
            DcmTag = item.Tag;
            TagDescription = item.Tag.DictionaryEntry.Name;
            _dcmVRCode = item.ValueRepresentation.Code;

            if (item is DicomSequence seq)
            {
                TagType = DcmTagType.Sequence;
                SequenceItems = new BindableCollection<DcmItem>();

                foreach (DicomDataset datasetItem in seq.Items)
                {
                    DcmItem seqItem = new DcmItem(datasetItem, seq)
                    {
                        DcmTag = DicomTag.Item,
                        TagDescription = $"{DicomTag.Item.DictionaryEntry.Name} #{SequenceItems.Count}",
                    };

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

                if (element is DicomOtherByte)
                {
                    // skip display large binary data
                    if (element.Count > 100)
                    {
                        TagValue = "[Binary Data]";
                        return;
                    }
                }

                // 校验是否合法
                try
                {
                    element.Validate();
                }
                catch (Exception)
                {
                    _isValid = false;
                }

                _tagValue = string.Join("\\", element.Get<string[]>());

                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    SetUidAdditionalInfo(element);
                }
            }
            else if (item is DicomFragmentSequence fragment)
            {
                if (fragment.Tag == DicomTag.PixelData)
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

        private DcmItem(DicomDataset dataset, DicomSequence sequence)
        {
            Dataset = dataset;
            Sequence = sequence;
            TagType = DcmTagType.SequenceItem;
            SequenceItems = new BindableCollection<DcmItem>();

            var enumerator = dataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SequenceItems.Add(new DcmItem(enumerator.Current, dataset));
            }
        }

        public void UpdateItem(DicomElement element)
        {
            DcmVRCode = element.ValueRepresentation.Code;
            IsValid = true;

            // 校验是否合法
            try
            {
                element.Validate();
            }
            catch (Exception)
            {
                IsValid = false;
            }

            if (element is DicomOtherByte)
            {
                // skip display large binary data
                if (element.Count > 100)
                {
                    TagValue = "[Binary Data]";
                    return;
                }
            }

            TagValue = string.Join("\\", element.Get<string[]>());

            if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
            {
                SetUidAdditionalInfo(element);
            }
        }

        private void SetUidAdditionalInfo(DicomElement element)
        {
            var uid = element.Get<DicomUID>(0);
            var name = uid.Name;
            if (name != "Unknown")
            {
                AdditionalInfo = $" ({name})";
            }
        }
    }
}

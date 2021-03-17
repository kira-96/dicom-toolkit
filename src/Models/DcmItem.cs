namespace SimpleDICOMToolkit.Models
{
#if FellowOakDicom5
    using FellowOakDicom;
#else
    using Dicom;
#endif
    using Stylet;
    using System;
    using System.Linq;

    public enum DicomItemType
    {
        Item,
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

        public DicomTag Tag { get; }

        private DicomVR _vr;

        public DicomVR VR
        {
            get => _vr;
            private set
            {
                if (SetAndNotify(ref _vr, value))
                {
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        private int _vm;

        public int VM
        {
            get => _vm;
            private set
            {
                if (SetAndNotify(ref _vm, value))
                {
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        private long _length;

        public long Length
        {
            get => _length;
            private set
            {
                if (SetAndNotify(ref _length, value))
                {
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        public string Description { get; }

        private string _values;

        public string Values
        {
            get => _values;
            private set
            {
                if (SetAndNotify(ref _values, value))
                {
                    NotifyOfPropertyChange(() => Content);
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

        public DicomItemType Type { get; } = DicomItemType.Item;

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
                return Type switch
                {
                    DicomItemType.Item => string.Format("({0:X4},{1:X4}) {2} {3} = ", Tag.Group, Tag.Element, VR, Description),
                    DicomItemType.Sequence => string.Format("({0:X4},{1:X4}) {2} {3}", Tag.Group, Tag.Element, VR, Description),
                    DicomItemType.SequenceItem => string.Format("({0:X4},{1:X4}) {2}", Tag.Group, Tag.Element, Description),
                    _ => string.Empty,
                };
            }
        }

        public string Content => Type == DicomItemType.Item ? string.Format("<{0}>", Values) : string.Empty;

        public BindableCollection<DcmItem> Items { get; }

        public DcmItem(DicomItem item, DicomDataset dataset)
        {
            Dataset = dataset;
            Tag = item.Tag;
            Description = item.Tag.DictionaryEntry.Name;
            _vr = item.ValueRepresentation;

            if (item is DicomSequence seq)
            {
                Type = DicomItemType.Sequence;
                Items = new BindableCollection<DcmItem>();

                foreach (DicomDataset itemDataset in seq.Items)
                {
                    DcmItem seqItem = new(itemDataset, seq, Items.Count);

                    Items.Add(seqItem);
                }
            }
            else if (item is DicomElement element)
            {
                _vm = element.Count;
                _length = element.Length;

                if (element.Tag == DicomTag.PixelData)
                {
                    _vm = 1;
                    _values = "[Binary Pixel Data]";
                    return;
                }

                if (element is DicomOtherByte)
                {
                    // skip display large binary data
                    if (element.Count > 100)
                    {
                        _values = "[Binary Data]";
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

                _values = string.Join("\\", element.Get<string[]>());

                if (element.ValueRepresentation == DicomVR.UI && element.Count > 0)
                {
                    SetUidAdditionalInfo(element);
                }
            }
            else if (item is DicomFragmentSequence fragment)
            {
                if (fragment.Tag == DicomTag.PixelData)
                {
                    _vm = fragment.Fragments.Count;
                    _length = fragment.Fragments.Sum(x => x.Size);
                    _values = "[Binary Pixel Data]";
                    return;
                }

                _values = "[Binary Data]";
            }
            else
            {
                // skip
            }
        }

        public DcmItem(DicomDataset dataset, DicomSequence sequence, int index)
        {
            Dataset = dataset;
            Sequence = sequence;
            Type = DicomItemType.SequenceItem;
            Tag = DicomTag.Item;
            Description = $"{DicomTag.Item.DictionaryEntry.Name} #{index}";
            Items = new BindableCollection<DcmItem>();

            var enumerator = dataset.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Items.Add(new DcmItem(enumerator.Current, dataset));
            }
        }

        public void UpdateItem(DicomElement element)
        {
            VR = element.ValueRepresentation;
            VM = element.Count;
            Length = element.Length;
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
                    Values = "[Binary Data]";
                    return;
                }
            }

            Values = string.Join("\\", element.Get<string[]>());

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

        public override string ToString()
        {
            return string.Format("{0} {1}", Tag, Description);
        }
    }
}

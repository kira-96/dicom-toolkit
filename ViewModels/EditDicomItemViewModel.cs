namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public class EditDicomItemViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator = SimpleIoC.Get<IEventAggregator>();

        private DicomDataset _currentDataset;

        private DicomTag _currentTag;

        private bool _isValuesChanged = false;

        public string TagText { get; private set; }

        public List<string> VRList { get; private set; }

        private string _currentVR;

        public string CurrentVR
        {
            get => _currentVR;
            set
            {
                if (SetAndNotify(ref _currentVR, value))
                {
                    _isValuesChanged = true;
                }
            }
        }

        public string _currentEditValue;

        public string CurrentEditValue
        {
            get => _currentEditValue;
            set => SetAndNotify(ref _currentEditValue, value);
        }

        private int _currentValueIndex = -1;

        public int CurrentValueIndex
        {
            get => _currentValueIndex;
            set
            {
                if (SetAndNotify(ref _currentValueIndex, value))
                {
                    if (value >= 0)
                    {
                        CurrentEditValue = ElementValues[value];
                    }
                    else
                    {
                        CurrentEditValue = "";
                    }
                }
            }
        }

        public BindableCollection<string> ElementValues { get; private set; }

        public EditDicomItemViewModel(DicomDataset dataset, DicomTag dicomTag)
        {
            Initialize();

            _currentDataset = dataset;
            _currentTag = dicomTag;
            DicomElement element = dataset.GetDicomItem<DicomElement>(dicomTag);

            DisplayName = dicomTag.DictionaryEntry.Name;
            TagText = string.Format("Tag: ({0:X4},{1:X4})", dicomTag.Group, dicomTag.Element);
            CurrentVR = element.ValueRepresentation.Code;

            for (int i = 0; i < element.Count; i++)
            {
                ElementValues.Add(element.Get<string>(i));
            }

            if (ElementValues.Count > 0)
            {
                CurrentValueIndex = 0;
            }
        }

        private void Initialize()
        {
            VRList = new List<string>()
            {
                "NONE",
                "AE", "AS", "AT",
                "CS",
                "DA", "DS", "DT",
                "FD", "FL", "IS",
                "LO", "LT",
                "OB", "OD", "OF", "OL", "OV", "OW",
                "PN",
                "SH", "SL", "SQ", "SS", "ST", "SV",
                "TM",
                "UC", "UI", "UL", "UN", "UR", "US", "UT", "UV"
            };
            ElementValues = new BindableCollection<string>();
        }

        public void UpdateCurrentValue()
        {
            if (CurrentValueIndex < 0)
            {
                InsertNewValue();
            }
            else
            {
                ElementValues[CurrentValueIndex] = CurrentEditValue;
            }

            CurrentEditValue = "";

            _isValuesChanged = true;
        }

        public void InsertNewValue()
        {
            ElementValues.Add(CurrentEditValue);

            CurrentEditValue = "";

            _isValuesChanged = true;
        }

        public void DeleteCurrentValue()
        {
            ElementValues.RemoveAt(CurrentValueIndex);

            CurrentEditValue = "";

            _isValuesChanged = true;
        }

        public void NotifyUpdateDicomItemValues()
        {
            if (_isValuesChanged)
            {
                _eventAggregator.Publish(new UpdateDicomElementItem(_currentDataset, DicomVR.Parse(CurrentVR), _currentTag, ElementValues.ToArray()));
            }
        }
    }
}

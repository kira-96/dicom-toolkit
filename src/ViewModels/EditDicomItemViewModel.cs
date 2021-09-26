namespace SimpleDICOMToolkit.ViewModels
{
    using FellowOakDicom;
    using Stylet;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Services;

    public class EditDicomItemViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly II18nService _i18NService;

        private DicomDataset _currentDataset;

        private DicomTag _currentTag;

        private bool _isValuesChanged = false;

        private bool _isEditItem = true;

        public bool IsEditItem
        {
            get => _isEditItem;
            private set => SetAndNotify(ref _isEditItem, value);
        }

        private string _tagString;

        public string TagString
        {
            get => _tagString;
            set
            {
                if (SetAndNotify(ref _tagString, value))
                {
                    _isValuesChanged = true;
                }
            }
        }

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

        public EditDicomItemViewModel(IEventAggregator eventAggregator, II18nService i18NService, IModelValidator<EditDicomItemViewModel> validator) : base(validator)
        {
            _eventAggregator = eventAggregator;
            _i18NService = i18NService;

            Initialize();
        }

        public void InitializeForAdd(DicomDataset dataset)
        {
            DisplayName = _i18NService.GetXmlStringByKey("Add");
            IsEditItem = false;
            TagString = DicomTag.Unknown.ToString("G", null);  // unknown
            CurrentVR = "UN";  // unknown
            _currentDataset = dataset;
        }

        public void InitializeForEdit(DicomDataset dataset, DicomTag dicomTag)
        {
            _currentDataset = dataset;
            _currentTag = dicomTag;
            DicomElement element = dataset.GetDicomItem<DicomElement>(dicomTag);

            DisplayName = dicomTag.DictionaryEntry.Name;
            TagString = dicomTag.ToString("G", null);
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
            if (IsEditItem)
            {
                if (_isValuesChanged)
                {
                    _eventAggregator.Publish(new UpdateDicomElementEvent(_currentDataset, DicomVR.Parse(CurrentVR), _currentTag, ElementValues.ToArray()));
                }
            }
            else
            {
                _eventAggregator.Publish(new AddDicomElementEvent(_currentDataset, DicomVR.Parse(CurrentVR), _tagString, ElementValues.ToArray()));
            }
        }

        public void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                RequestClose();
            }
        }

        public void OnOK()
        {
            NotifyUpdateDicomItemValues();
            RequestClose(true);
        }

        public void OnCancel()
        {
            RequestClose(false);
        }
    }
}

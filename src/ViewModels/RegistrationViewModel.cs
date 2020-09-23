namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using System;
    using System.Linq;
    using Models;
    using Validators;

    public class RegistrationViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        private bool _canEdit = true;

        public bool CanEdit
        {
            get => _canEdit;
            set
            {
                if (SetAndNotify(ref _canEdit, value))
                {
                    NotifyOfPropertyChange(() => CanRegist);
                }
            }
        }

        private string _hospitalName = "";

        public string HospitalName
        {
            get => _hospitalName;
            set => SetAndNotify(ref _hospitalName, value);
        }

        private string _examRoom = "";

        public string ExamRoom
        {
            get => _examRoom;
            set => SetAndNotify(ref _examRoom, value);
        }

        private string _referringPhysicianName = "";

        public string ReferringPhysicianName
        {
            get => _referringPhysicianName;
            set => SetAndNotify(ref _referringPhysicianName, value);
        }

        private string _patientName;

        public string PatientName
        {
            get => _patientName;
            set => SetAndNotify(ref _patientName, value);
        }

        private string _patientId = "";

        public string PatientID
        {
            get => _patientId;
            set => SetAndNotify(ref _patientId, value);
        }

        private string _accessionNumber = "";

        public string AccessionNumber
        {
            get => _accessionNumber;
            set => SetAndNotify(ref _accessionNumber, value);
        }

        public string[] AvailableSex { get; } = new[] { "F", "M", "O" };

        private string _sex = "O";

        public string Sex
        {
            get => _sex;
            set => SetAndNotify(ref _sex, value);
        }

        private string _age = "";

        public string Age
        {
            get => _age;
            set => SetAndNotify(ref _age, value);
        }

        private string _birthDate = "";

        public string BirthDate
        {
            get => _birthDate;
            set => SetAndNotify(ref _birthDate, value);
        }

        private string _modality = "";

        public string Modality
        {
            get => _modality;
            set => SetAndNotify(ref _modality, value);
        }

        private string _scheduledAET = "";

        public string ScheduledAET
        {
            get => _scheduledAET;
            set => SetAndNotify(ref _scheduledAET, value);
        }

        private string _scheduledDate = "";

        public string ScheduledDate
        {
            get => _scheduledDate;
            set => SetAndNotify(ref _scheduledDate, value);
        }

        private string _performingPhysicianName = "";

        public string PerformingPhysicianName
        {
            get => _performingPhysicianName;
            set => SetAndNotify(ref _performingPhysicianName, value);
        }

        private string _description = "";

        public string Description
        {
            get => _description;
            set => SetAndNotify(ref _description, value);
        }

        public RegistrationViewModel(IEventAggregator eventAggregator, IModelValidator<RegistrationViewModel> validator) : base(validator)
        {
            _eventAggregator = eventAggregator;

            PatientName = "";
            BirthDate = DateTime.Today.AddYears(-24).ToString("yyyyMMdd");
            ScheduledDate = DateTime.Today.ToString("yyyyMMdd");
            ExamRoom = "MR";
            Modality = "MR";
            ScheduledAET = "ANY-SCU";
            Age = "024Y";
            Random rand = new Random(DateTime.Now.Millisecond);
            PatientID = $"P{rand.Next(1, 1000)}";
            AccessionNumber = PatientID;
        }

        public void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                RequestClose();
            }
        }

        public bool CanRegist => CanEdit;

        public void Regist()
        {
            WorklistItem item = new WorklistItem()
            {
                AccessionNumber = _accessionNumber,
                PatientID = _patientId,
                PatientName = _patientName,
                Sex = _sex,
                Age = _age,
                ReferringPhysician = _referringPhysicianName,
                PerformingPhysician = _performingPhysicianName,
                Modality = _modality,
                ExamRoom = _examRoom,
                ExamDescription = _description,
                HospitalName = _hospitalName,
                ScheduledAET = _scheduledAET,
                StudyUID = DicomUIDGenerator.GenerateDerivedFromUUID().UID,
                ProcedureID = GenerateShFromGUID(),
                ProcedureStepID = GenerateShFromGUID(),
            };

            try
            {
                DateTime birthDate = DateTime.ParseExact(_birthDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                item.DateOfBirth = birthDate;
                DateTime scheduledDate = DateTime.ParseExact(_scheduledDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                item.ExamDateAndTime = scheduledDate;

                _eventAggregator.Publish(item, nameof(PatientsViewModel));

                RequestClose();
            }
            finally
            {}
        }

        private string GenerateShFromGUID()
        {
            Guid guid = Guid.NewGuid();
            string val = guid.ToString().Split('-').Last();
            Random rand = new Random();

            return $"{val}{rand.Next(100, 999)}";
        }
    }
}

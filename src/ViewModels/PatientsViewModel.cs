namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.ObjectModel;
    using Models;
    using Server;
    using Services;
    using Helpers;

    public class PatientsViewModel : Screen, IHandle<ServerMessageItem>, IHandle<WorklistItem>, IDisposable
    {
        [Inject]
        private IWindowManager _windowManager;

        [Inject]
        private II18nService i18NService;

        [Inject]
        private IViewModelFactory viewModelFactory;

        [Inject]
        private INotificationService notificationService;

        private readonly IEventAggregator _eventAggregator;

        private WorklistItemsSource _worklistItemsSource;

        public ObservableCollection<WorklistItem> WorklistItems => _worklistItemsSource.WorklistItems;

        private bool _isServerStarted = false;

        public bool IsServerStarted
        {
            get => _isServerStarted;
            set => SetAndNotify(ref _isServerStarted, value);
        }

        public PatientsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(PatientsViewModel));

            _worklistItemsSource = new WorklistItemsSource();
            WorklistServer.Default.ItemsSource = _worklistItemsSource;

#if DEBUG
            _worklistItemsSource.AddItem(
                new WorklistItem()
                {
                    PatientName = "Test1",
                    PatientID = "001",
                    Age = "027Y",
                    Sex = "M",
                    DateOfBirth = DateTime.Today,
                    StudyUID = "1.23.456.7890.1234567890.1",
                    Modality = "MR",
                    ScheduledAET = "LOCAL_AET",
                    ExamDateAndTime = DateTime.Today,
                    ProcedureID = "DDB620C9F6D7101",
                    ProcedureStepID = "4C599B2EEBB0102"
                });
            _worklistItemsSource.AddItem(
                new WorklistItem()
                {
                    PatientName = "Test2",
                    PatientID = "002",
                    Age = "028Y",
                    Sex = "F",
                    DateOfBirth = DateTime.Today,
                    StudyUID = "1.23.456.7890.1234567890.2",
                    Modality = "MR",
                    ScheduledAET = "LOCAL_AET",
                    ExamDateAndTime = DateTime.Today,
                    ProcedureID = "779B2791AA56103",
                    ProcedureStepID = "0F4BF697489C104"
                });
#endif
        }

        public void Handle(ServerMessageItem message)
        {
            if (IsServerStarted)
            {
                WorklistServer.Default.CreateServer(message.ServerPort, message.LocalAET);
                _eventAggregator.Publish(new ServerStateItem(true), nameof(PatientsViewModel));
                notificationService.ShowNotification(
                    string.Format(i18NService.GetXmlStringByKey("ServerIsRunning"), "Worklist", SystemHelper.LocalIPAddress, message.ServerPort), 
                    message.LocalAET);
                WorklistServer.Default.IsListening();
            }
            else
            {
                WorklistServer.Default.StopServer();
                _eventAggregator.Publish(new ServerStateItem(false), nameof(PatientsViewModel));
            }
        }

        public void ShowRegistrationWindow()
        {
            var register = viewModelFactory.GetRegistrationViewModel();

            _windowManager.ShowDialog(register, this);
        }

        public void Handle(WorklistItem message)
        {
            _worklistItemsSource.AddItem(message);
        }

        public void RemoveItem(WorklistItem item)
        {
            _worklistItemsSource.RemoveItem(item);
        }

        public void ViewDetails(WorklistItem item)
        {
            var register = viewModelFactory.GetRegistrationViewModel();

            register.CanEdit = false;
            register.HospitalName = item.HospitalName;
            register.ExamRoom = item.ExamRoom;
            register.ReferringPhysicianName = item.ReferringPhysician;
            register.PatientName = item.PatientName;
            register.PatientID = item.PatientID;
            register.AccessionNumber = item.AccessionNumber;
            register.Sex = item.Sex;
            register.Age = item.Age;
            register.BirthDate = item.DateOfBirth.ToString("yyyyMMdd");
            register.Modality = item.Modality;
            register.ScheduledAET = item.ScheduledAET;
            register.ScheduledDate = item.ExamDateAndTime.ToString("yyyyMMdd");
            register.PerformingPhysicianName = item.PerformingPhysician;
            register.Description = item.ExamDescription;

            _windowManager.ShowDialog(register, this);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);

            WorklistServer.Default.StopServer();
        }
    }
}

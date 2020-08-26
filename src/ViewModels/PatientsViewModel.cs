namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
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

        [Inject]
        private IDataService dataService;

        private readonly IEventAggregator _eventAggregator;

        public BindableCollection<WorklistItem> WorklistItems { get; }

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

            WorklistItems = new BindableCollection<WorklistItem>();
            WorklistServer.Default.WorklistItems = WorklistItems;
        }

        public void UpdateData()
        {
            WorklistItems.AddRange(dataService.GetWorklistItems());
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
            WorklistItems.Add(message);
            dataService.AddWorklistItem(message);
        }

        public void RemoveItem(WorklistItem item)
        {
            WorklistItems.Remove(item);
            dataService.RemoveWorklistItem(item);
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

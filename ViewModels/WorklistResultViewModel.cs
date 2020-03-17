namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using Client;
    using Models;

    public class WorklistResultViewModel : Screen, IHandle<WorklistRequestItem>
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWorklistSCU _worklistSCU;

        [Inject]
        private IViewModelFactory _viewModelFactory;  // 抽象工厂模式

        // 由于只是简单测试，这里只做临时保存
        private Dicom.DicomUID AffectedInstanceUID = null;
        private string StudyInstanceUID = null;

        public BindableCollection<SimpleWorklistResult> WorklistItems { get; private set; }

        public WorklistResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            WorklistItems = new BindableCollection<SimpleWorklistResult>();
        }

        public async void StartPerformance(SimpleWorklistResult item)
        {
            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = _viewModelFactory.GetWorklistViewModel().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            var result = await _worklistSCU.SendMppsInProgressAsync(config.ServerIP, port, config.ServerAET, config.LocalAET, dataset);

            if (result.result)
            {
                AffectedInstanceUID = result.affectedInstanceUid;
                StudyInstanceUID = result.studyInstanceUid;
            }
        }

        public async void CompletePerformance(SimpleWorklistResult item)
        {
            if (AffectedInstanceUID == null ||
                StudyInstanceUID == null)
            {
                return;
            }

            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = _viewModelFactory.GetWorklistViewModel().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            await _worklistSCU.SenMppsCompletedAsync(config.ServerIP, port, config.ServerAET, config.LocalAET, StudyInstanceUID, AffectedInstanceUID, dataset);

            AffectedInstanceUID = null;
            StudyInstanceUID = null;
        }

        public async void Handle(WorklistRequestItem message)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(WorklistResultViewModel));

            WorklistItems.Clear();

            var result = await _worklistSCU.GetAllResultFromWorklistAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, message.Modality);

            WorklistItems.AddRange(result);

            _eventAggregator.Publish(new BusyStateItem(false), nameof(WorklistResultViewModel));
        }

        protected override void OnClose()
        {
            _eventAggregator.Unsubscribe(this);

            base.OnClose();
        }
    }
}

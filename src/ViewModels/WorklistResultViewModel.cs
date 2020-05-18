namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Client;
    using Models;

    public class WorklistResultViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

#pragma warning disable IDE0044, 0649
        [Inject]
        private IWorklistSCU _worklistSCU;
#pragma warning disable IDE0044, 0649

        // 由于只是简单测试，这里只做临时保存
        private Dicom.DicomUID AffectedInstanceUID = null;
        private string StudyInstanceUID = null;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetAndNotify(ref _isBusy, value);
        }

        public BindableCollection<SimpleWorklistResult> WorklistItems { get; private set; }

        public WorklistResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(WorklistResultViewModel));
            WorklistItems = new BindableCollection<SimpleWorklistResult>();
        }

        public async void StartPerformance(SimpleWorklistResult item)
        {
            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = SimpleIoC.Get<WorklistViewModel>().ServerConfigViewModel;

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

            var config = SimpleIoC.Get<WorklistViewModel>().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            await _worklistSCU.SenMppsCompletedAsync(config.ServerIP, port, config.ServerAET, config.LocalAET, StudyInstanceUID, AffectedInstanceUID, dataset);

            AffectedInstanceUID = null;
            StudyInstanceUID = null;
        }

        public async void Handle(ClientMessageItem message)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(WorklistResultViewModel));
            IsBusy = true;

            WorklistItems.Clear();

            var result = await _worklistSCU.GetAllResultFromWorklistAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, message.Modality);

            WorklistItems.AddRange(result);

            IsBusy = false;
            _eventAggregator.Publish(new BusyStateItem(false), nameof(WorklistResultViewModel));
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

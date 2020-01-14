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

        public BindableCollection<SimpleWorklistResult> WorklistItems { get; private set; }

        public WorklistResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            WorklistItems = new BindableCollection<SimpleWorklistResult>();
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

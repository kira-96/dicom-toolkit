namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Threading.Tasks;
    using Helpers;

    public class WorklistSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PatientsViewModel PatientsViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public WorklistSCPViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Worklist SCP";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            PatientsViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishServerRequest(nameof(ViewModels.PatientsViewModel));
            ServerConfigViewModel.ServerPort = "6104";
            ServerConfigViewModel.LocalAET = ServerConfigViewModel.ServerAET = "RIS";
            ServerConfigViewModel.IsServerIPEnabled = ServerConfigViewModel.IsServerAETEnabled = ServerConfigViewModel.IsModalityEnabled = false;
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.PatientsViewModel));
            Task.Run(() =>
            {
                Execute.OnUIThread(() =>
                {
                    ServerConfigViewModel.ServerIP = SystemHelper.GetLocalIPAddress();
                    PatientsViewModel.UpdateData();
                });
            });
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            PatientsViewModel.Dispose();
        }
    }
}

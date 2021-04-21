namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Helpers;

    public class StoreSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public StoreReceivedViewModel StoreReceivedViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public StoreSCPViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "C-STORE SCP";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            StoreReceivedViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerIP = SystemHelper.GetLocalIPAddress();
            ServerConfigViewModel.ServerPort = "104";
            ServerConfigViewModel.LocalAET = ServerConfigViewModel.ServerAET = "STORE-SCP";
            ServerConfigViewModel.IsServerIPEnabled = ServerConfigViewModel.IsServerAETEnabled = ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishServerRequest(nameof(ViewModels.StoreReceivedViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.StoreReceivedViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            StoreReceivedViewModel.Dispose();
        }
    }
}

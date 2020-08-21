namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Helpers;

    public class CStoreSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public CStoreReceivedViewModel CStoreReceivedViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public CStoreSCPViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "C-Store SCP";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            CStoreReceivedViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerIP = SystemHelper.LocalIPAddress;
            ServerConfigViewModel.ServerPort = "104";
            ServerConfigViewModel.LocalAET = ServerConfigViewModel.ServerAET = "CSTORESCP";
            ServerConfigViewModel.IsServerIPEnabled = ServerConfigViewModel.IsServerAETEnabled = ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishServerRequest(nameof(ViewModels.CStoreReceivedViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.CStoreReceivedViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            CStoreReceivedViewModel.Dispose();
        }
    }
}

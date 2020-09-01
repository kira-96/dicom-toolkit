namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class StoreViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public StoreFileListViewModel StoreFileListViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public StoreViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "C-STORE";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            StoreFileListViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerPort = "104";
            ServerConfigViewModel.ServerAET = "STORE-SCP";
            ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishClientRequest(nameof(ViewModels.StoreFileListViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.StoreFileListViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            StoreFileListViewModel.Dispose();
        }
    }
}

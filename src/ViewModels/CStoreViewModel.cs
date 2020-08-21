namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class CStoreViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public CStoreFileListViewModel CStoreFileListViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public CStoreViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "C-Store";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            CStoreFileListViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerPort = "104";
            ServerConfigViewModel.ServerAET = "CSTORESCP";
            ServerConfigViewModel.IsModalityEnabled = false;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishClientRequest(nameof(ViewModels.CStoreFileListViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.CStoreFileListViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            CStoreFileListViewModel.Dispose();
        }
    }
}

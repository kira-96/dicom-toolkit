namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class QueryRetrieveViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public QueryResultViewModel QueryResultViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public QueryRetrieveViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Query";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            QueryResultViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.ServerIP = "www.dicomserver.co.uk";
            ServerConfigViewModel.ServerPort = "104";  // 104/11112
            ServerConfigViewModel.ServerAET = "QRSCP";
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishClientRequest(nameof(ViewModels.QueryResultViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.QueryResultViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            QueryResultViewModel.Dispose();
        }
    }
}

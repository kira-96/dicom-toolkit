namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class WorklistViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public WorklistResultViewModel WorklistResultViewModel { get; private set; }

        private readonly IEventAggregator eventAggregator;

        public WorklistViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Worklist";
            this.eventAggregator = eventAggregator;
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            WorklistResultViewModel.Parent = this;
            ServerConfigViewModel.Parent = this;
            ServerConfigViewModel.RequestAction = () => ServerConfigViewModel.PublishClientRequest(nameof(ViewModels.WorklistResultViewModel));
            eventAggregator.Subscribe(ServerConfigViewModel, nameof(ViewModels.WorklistResultViewModel));
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            WorklistResultViewModel.Dispose();
        }
    }
}

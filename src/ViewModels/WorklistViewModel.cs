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

        public WorklistViewModel()
        {
            DisplayName = "Worklist";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
            WorklistResultViewModel.Parent = this;
        }

        public void Dispose()
        {
            // TODO
            ServerConfigViewModel.Dispose();
            WorklistResultViewModel.Dispose();
        }
    }
}

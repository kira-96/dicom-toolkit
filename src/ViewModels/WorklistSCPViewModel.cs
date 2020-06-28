namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class WorklistSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PatientsViewModel PatientsViewModel { get; private set; }

        public WorklistSCPViewModel()
        {
            DisplayName = "Worklist SCP";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            ServerConfigViewModel.Init(this);
            PatientsViewModel.Parent = this;
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            PatientsViewModel.Dispose();
        }
    }
}

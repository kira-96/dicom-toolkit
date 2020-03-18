namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class PrintSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PrintJobsViewModel PrintJobsViewModel { get; private set; }

        public PrintSCPViewModel()
        {
            DisplayName = "Print SCP";
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            ServerConfigViewModel.Init(this);
        }

        public void Dispose()
        {
            // TODO
        }
    }
}

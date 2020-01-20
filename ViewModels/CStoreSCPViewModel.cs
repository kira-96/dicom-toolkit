namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class CStoreSCPViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public CStoreReceivedViewModel CStoreReceivedViewModel { get; private set; }

        public CStoreSCPViewModel()
        {
            DisplayName = "C-Store SCP";
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

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

        public CStoreViewModel()
        {
            DisplayName = "C-Store";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
            CStoreFileListViewModel.Parent = this;
        }

        public void Dispose()
        {
            // TODO
            ServerConfigViewModel.Dispose();
            CStoreFileListViewModel.Dispose();
        }
    }
}

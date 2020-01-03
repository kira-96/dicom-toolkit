namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;

    public class PrintViewModel : Screen, IDisposable
    {
        [Inject]
        public ServerConfigViewModel ServerConfigViewModel { get; private set; }

        [Inject]
        public PrintPreviewViewModel PrintPreviewViewModel { get; private set; }

        public PrintViewModel()
        {
            DisplayName = "Print";
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

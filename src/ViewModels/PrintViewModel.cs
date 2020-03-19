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

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
        }

        public void Dispose()
        {
            // TODO
            ServerConfigViewModel.Dispose();
            PrintPreviewViewModel.Dispose();
        }
    }
}

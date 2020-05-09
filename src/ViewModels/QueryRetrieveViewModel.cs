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

        public QueryRetrieveViewModel()
        {
            DisplayName = "Query";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();
            ServerConfigViewModel.Init(this);
            QueryResultViewModel.Parent = this;
        }

        public void Dispose()
        {
            ServerConfigViewModel.Dispose();
            QueryResultViewModel.Dispose();
        }
    }
}

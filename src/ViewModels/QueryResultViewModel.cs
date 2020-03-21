namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using Client;
    using Models;

    public class QueryResultViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IQueryRetrieveSCU queryRetrieveSCU;

        [Inject]
        private IViewModelFactory viewModelFactory;

        public QueryResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(QueryResultViewModel));
        }

        public async void Handle(ClientMessageItem message)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));

            // test only
            var result = await queryRetrieveSCU.GetImagesBySeriesAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, 
                "1.2.826.0.1.3680043.2.1125.205754746405378877375542647950067512",
                "1.2.826.0.1.3680043.2.1125.4805032840907332017522988287174134246");

            for (int i = 0; i < result.Count; i++)
            {
                Dicom.DicomFile file = new Dicom.DicomFile(result[i]);
                await file.SaveAsync("test.dcm");
            }
            //

            _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

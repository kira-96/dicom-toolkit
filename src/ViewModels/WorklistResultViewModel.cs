namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using Client;
    using Models;

    public class WorklistResultViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWorklistSCU _worklistSCU;

        private Dictionary<string, Dicom.DicomUID> _affectedUidDict;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetAndNotify(ref _isBusy, value);
        }

        public BindableCollection<SimpleWorklistResult> WorklistItems { get; private set; }

        public WorklistResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(WorklistResultViewModel));
            _affectedUidDict = new Dictionary<string, Dicom.DicomUID>();
            WorklistItems = new BindableCollection<SimpleWorklistResult>();
        }

        public async void StartPerformance(SimpleWorklistResult item)
        {
            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = SimpleIoC.Get<WorklistViewModel>().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            try
            {
                var result = await _worklistSCU.SendMppsInProgressAsync(config.ServerIP, port, config.ServerAET, config.LocalAET, dataset);

                if (result.result)
                {
                    if (_affectedUidDict.ContainsKey(result.studyInstanceUid))
                        _affectedUidDict[result.studyInstanceUid] = result.affectedInstanceUid;
                    else
                        _affectedUidDict.Add(result.studyInstanceUid, result.affectedInstanceUid);
                }
            }
            finally
            {}
        }

        public async void DiscontinuedPerformance(SimpleWorklistResult item)
        {
            if (!_affectedUidDict.ContainsKey(item.StudyUID))
            {
                return;
            }

            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = SimpleIoC.Get<WorklistViewModel>().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            try
            {
                await _worklistSCU.SendMppsDiscontinuedAsync(config.ServerIP, port, config.ServerAET, config.LocalAET, 
                    item.StudyUID, _affectedUidDict[item.StudyUID], dataset);

                _affectedUidDict.Remove(item.StudyUID);
            }
            finally
            {}
        }

        public async void CompletePerformance(SimpleWorklistResult item)
        {
            if (!_affectedUidDict.ContainsKey(item.StudyUID))
            {
                return;
            }

            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByPID(item.PatientId);

            var config = SimpleIoC.Get<WorklistViewModel>().ServerConfigViewModel;

            int port = config.ParseServerPort();
            if (port == 0) return;

            try
            {
                await _worklistSCU.SendMppsCompletedAsync(config.ServerIP, port, config.ServerAET, config.LocalAET,
                    item.StudyUID, _affectedUidDict[item.StudyUID], dataset);

                _affectedUidDict.Remove(item.StudyUID);
            }
            finally
            {}
        }

        public async void Handle(ClientMessageItem message)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(WorklistResultViewModel));
            IsBusy = true;

            WorklistItems.Clear();

            try
            {
                var result = await _worklistSCU.GetAllResultFromWorklistAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, message.Modality);

                WorklistItems.AddRange(result);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(WorklistResultViewModel));
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

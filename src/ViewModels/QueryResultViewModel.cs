namespace SimpleDICOMToolkit.ViewModels
{
#if FellowOakDicom5
    using FellowOakDicom;
#else
    using Dicom;
#endif
    using Polly;
    using Polly.Timeout;
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Infrastructure;
    using Logging;
    using Models;

    public class QueryResultViewModel : Screen, IHandle<DicomRequestEvent>, IDisposable
    {
        private const int TimeoutTime = 180;
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IWindowManager _windowManager;

        [Inject("filelogger")]
        private ILoggerService _logger;

        [Inject]
        private IViewModelFactory _viewModelFactory;

        [Inject]
        private IQueryRetrieveSCU queryRetrieveSCU;

        public BindableCollection<IDicomObjectLevel> QueryResult { get; } = new BindableCollection<IDicomObjectLevel>();

        private IDicomObjectLevel selectedPatient;
        private IDicomObjectLevel selectedStudy;
        private IDicomObjectLevel selectedSeries;
        private IDicomObjectLevel selectedImage;

        public IDicomObjectLevel SelectedPatient
        {
            get => selectedPatient;
            set
            {
                if (SetAndNotify(ref selectedPatient, value))
                {
                    if (selectedPatient != null && !selectedPatient.HasChildren)
                    {
                        _ = QueryStudiesAsync(selectedPatient);
                    }
                }
            }
        }

        public IDicomObjectLevel SelectedStudy
        {
            get => selectedStudy;
            set
            {
                if (SetAndNotify(ref selectedStudy, value))
                {
                    if (selectedStudy != null && !selectedStudy.HasChildren)
                    {
                        _ = QuerySeriesAsync(selectedStudy);
                    }
                }
            }
        }

        public IDicomObjectLevel SelectedSeries
        {
            get => selectedSeries;
            set
            {
                if (SetAndNotify(ref selectedSeries, value))
                {
                    if (selectedSeries != null && !selectedSeries.HasChildren)
                    {
                        _ = QueryImagesAsync(selectedSeries);
                    }
                }
            }
        }

        public IDicomObjectLevel SelectedImage
        {
            get => selectedImage;
            set => SetAndNotify(ref selectedImage, value);
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetAndNotify(ref _isBusy, value))
                {
                    NotifyOfPropertyChange(() => CanSelectItem);
                }
            }
        }

        public bool CanSelectItem => !IsBusy;

        public QueryResultViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this, nameof(QueryResultViewModel));
        }

        public async void Handle(DicomRequestEvent message)
        {
            await QueryPatientsAsync(message);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }

        public async void MoveStudyAsync(IDicomObjectLevel obj)
        {
            MoveToViewModel moveTo = _viewModelFactory.GetMoveToViewModel();

            if (_windowManager.ShowDialog(moveTo, this) != true)
            {
                return;
            }

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            try
            {
                await queryRetrieveSCU.MoveImagesAsync(serverIp, serverPort, serverAet, localAet, moveTo.ServerAET, obj.UID, null);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }
        }

        public async ValueTask PreviewImageAsync()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var timeoutPolicy = GetTimeoutPolicy();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();
            DicomDataset result = null;

            try
            {
                result = await timeoutPolicy.ExecuteAsync(async () =>
                {
                    return await queryRetrieveSCU.GetImagesBySOPInstanceAsync(
                        serverIp, serverPort, serverAet, localAet,
                        selectedImage.Parent.Parent.UID, selectedImage.Parent.UID, selectedImage.UID, cancellationTokenSource.Token);
                });
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }

            // 有时候查询到的图像没有像素值，无法显示
            if (result != null && result.Contains(DicomTag.PixelData))
            {
                var preview = _viewModelFactory.GetPreviewImageViewModel();
                preview.Initialize(result);
                _windowManager.ShowDialog(preview, this);
            }
        }

        private (string serverIp, int serverPort, string serverAet, string localAet) GetServerConfig()
        {
            var configVm = (Parent as QueryRetrieveViewModel).ServerConfigViewModel;
            return (configVm.ServerIP, configVm.ParseServerPort(), configVm.ServerAET, configVm.LocalAET);
        }

        private AsyncTimeoutPolicy GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync(TimeoutTime, TimeoutStrategy.Pessimistic,
                (context, timespan, abandonedTask) =>
                {
                    // ContinueWith important!: the abandoned task may very well still be executing, when the caller times out on waiting for it!
                    abandonedTask.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.Error($"{context.PolicyKey} at {context.OperationKey}: execution timed out after {timespan.TotalSeconds} seconds, eventually terminated with: {t.Exception}.");
                        }
                        else if (t.IsCanceled)
                        {
                            // (If the executed delegates do not honour cancellation, this IsCanceled branch may never be hit.  It can be good practice however to include, in case a Policy configured with TimeoutStrategy.Pessimistic is used to execute a delegate honouring cancellation.)
                        }
                        else
                        {
                            // extra logic (if desired) for tasks which complete, despite the caller having 'walked away' earlier due to timeout.
                        }
                    });

                    return Task.FromResult(true);
                });
        }

        private async ValueTask QueryPatientsAsync(DicomRequestEvent message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var timeoutPolicy = GetTimeoutPolicy();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            IEnumerable<DicomDataset> result = null;

            try
            {
                result = await timeoutPolicy.ExecuteAsync(async () =>
                {
                    return await queryRetrieveSCU.QueryPatientsAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, null, null, cancellationTokenSource.Token);
                });
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }

            if (result != null)
            {
                foreach (DicomDataset item in result)
                {
                    string name = item.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);
                    string pid = item.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty);
                    if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(pid))
                    {
                        if (string.IsNullOrEmpty(name)) name = pid;
                        DicomObjectLevel objectLevel = new DicomObjectLevel(name, pid, Level.Patient, null);
                        QueryResult.Add(objectLevel);
                    }
                }
            }
        }

        private async ValueTask QueryStudiesAsync(IDicomObjectLevel obj)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var timeoutPolicy = GetTimeoutPolicy();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            IEnumerable<DicomDataset> result = null;

            try
            {
                result = await timeoutPolicy.ExecuteAsync(async () =>
                {
                    return await queryRetrieveSCU.QueryStudiesByPatientAsync(serverIp, serverPort, serverAet, localAet, obj.UID, null, null, cancellationTokenSource.Token);
                });
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }

            if (result != null)
            {
                foreach (DicomDataset item in result)
                {
                    string studyDate = item.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty);
                    string studyUid = item.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

                    if (!string.IsNullOrEmpty(studyDate) || !string.IsNullOrEmpty(studyUid))
                    {
                        if (string.IsNullOrEmpty(studyDate))
                        {
                            studyDate = studyUid;
                        }
                        obj.Children.Add(new DicomObjectLevel(studyDate, studyUid, Level.Study, obj));
                    }
                }
            }
        }

        private async ValueTask QuerySeriesAsync(IDicomObjectLevel obj)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var timeoutPolicy = GetTimeoutPolicy();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            IEnumerable<DicomDataset> result = null;

            try
            {
                result = await timeoutPolicy.ExecuteAsync(async () => { return await queryRetrieveSCU.QuerySeriesByStudyAsync(serverIp, serverPort, serverAet, localAet, obj.UID, null, cancellationTokenSource.Token); });
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }

            if (result != null)
            {
                foreach (DicomDataset item in result)
                {
                    string seriesNumber = item.GetSingleValueOrDefault(DicomTag.SeriesNumber, string.Empty);
                    string seriesUid = item.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);

                    if (!string.IsNullOrEmpty(seriesNumber) || !string.IsNullOrEmpty(seriesUid))
                    {
                        if (string.IsNullOrEmpty(seriesNumber))
                        {
                            seriesNumber = seriesUid;
                        }
                        obj.Children.Add(new DicomObjectLevel(seriesNumber, seriesUid, Level.Series, obj));
                    }
                }
            }
        }

        private async ValueTask QueryImagesAsync(IDicomObjectLevel obj)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var timeoutPolicy = GetTimeoutPolicy();

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            IEnumerable<DicomDataset> result = null;
            try
            {
                result = await timeoutPolicy.ExecuteAsync(async () =>
                {
                    return await queryRetrieveSCU.QueryImagesByStudyAndSeriesAsync(
                        serverIp, serverPort, serverAet, localAet, obj.Parent.UID, obj.UID, null, cancellationTokenSource.Token);
                });
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(QueryResultViewModel));
            }

            if (result != null)
            {
                foreach (DicomDataset item in result)
                {
                    string instanceNumber = item.GetSingleValueOrDefault(DicomTag.InstanceNumber, string.Empty);
                    string instanceUid = item.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty);

                    if (!string.IsNullOrEmpty(instanceNumber) || !string.IsNullOrEmpty(instanceUid))
                    {
                        if (string.IsNullOrEmpty(instanceNumber))
                        {
                            instanceNumber = instanceUid;
                        }
                        obj.Children.Add(new DicomObjectLevel(instanceNumber, instanceUid, Level.Image, obj));
                    }
                }
            }
        }
    }
}

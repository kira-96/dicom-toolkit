namespace SimpleDICOMToolkit.ViewModels
{
#if FellowOakDicom5
    using FellowOakDicom;
#else
    using Dicom;
#endif
    using Polly;
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Infrastructure;
    using Logging;
    using Models;
    using Services;

    public class WorklistResultViewModel : Screen, IHandle<DicomRequestEvent>, IDisposable
    {
        private const int TimeoutTime = 30;
        private readonly IEventAggregator _eventAggregator;

        [Inject("filelogger")]
        private ILoggerService _logger;

        [Inject]
        private IConfigurationService _configurationService;

        [Inject]
        private II18nService _i18NService;

        [Inject]
        private INotificationService _notificationService;

        [Inject]
        private IWorklistSCU _worklistSCU;

        private Dictionary<string, DicomUID> _affectedUidDict;

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
            _affectedUidDict = new Dictionary<string, DicomUID>();
            WorklistItems = new BindableCollection<SimpleWorklistResult>();
        }

        public async void StartPerformance(SimpleWorklistResult item)
        {
            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByStudyUid(item.StudyUID);

            if (dataset == null)
            {
                return;
            }

            var config = (Parent as WorklistViewModel).ServerConfigViewModel;

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

            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByStudyUid(item.StudyUID);

            if (dataset == null)
            {
                return;
            }

            var config = (Parent as WorklistViewModel).ServerConfigViewModel;

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

            var dataset = (_worklistSCU as WorklistSCU).GetWorklistItemByStudyUid(item.StudyUID);

            if (dataset == null)
            {
                return;
            }

            var config = (Parent as WorklistViewModel).ServerConfigViewModel;

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

        public async void Handle(DicomRequestEvent message)
        {
            var timeoutPolicy = Policy.TimeoutAsync(TimeoutTime, Polly.Timeout.TimeoutStrategy.Pessimistic, 
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

            IsBusy = true;
            _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(WorklistResultViewModel));

            WorklistItems.Clear();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Alt 1
                //var result = await timeoutPolicy.ExecuteAsync(async () =>
                //{
                //    Encoding fallbackEncoding = Dicom.DicomEncoding.Default;  // 不要移除这行代码，.NET Core 平台会在这里注册 CodePagesEncodingProvider
                //    fallbackEncoding = Encoding.GetEncoding(_configurationService.GetConfiguration<AppConfiguration>().DicomEncoding);
                //    return await _worklistSCU.GetAllResultFromWorklistAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, message.Modality, fallbackEncoding, cancellationTokenSource.Token);
                //});

                //WorklistItems.AddRange(result);

                // Alt 2
                await timeoutPolicy.ExecuteAsync(async () =>
                {
                    Encoding fallbackEncoding = DicomEncoding.Default;  // 不要移除这行代码，.NET Core 平台会在这里注册 CodePagesEncodingProvider
                    fallbackEncoding = Encoding.GetEncoding(_configurationService.Get<MiscConfiguration>().DicomEncoding);
                    await _worklistSCU.GetAllResultFromWorklistAsync(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET, WorklistItems, message.Modality, fallbackEncoding, cancellationTokenSource.Token);
                });

                string content = string.Format(
                    _i18NService.GetXmlStringByKey("ToastWorklistResult"),
                    _i18NService.GetXmlStringByKey("Success"));
                // 这里不使用 await，否则当前线程会阻塞直到toast显示完成
                _ = _notificationService.ShowToastAsync(content, new TimeSpan(0, 0, 3));
            }
            finally
            {
                cancellationTokenSource.Cancel();
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateEvent(IsBusy), nameof(WorklistResultViewModel));
            }
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

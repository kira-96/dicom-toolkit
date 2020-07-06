namespace SimpleDICOMToolkit.ViewModels
{
    using Dicom;
    using Stylet;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client;
    using Models;

    public class QueryResultViewModel : Screen, IHandle<ClientMessageItem>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        private IQueryRetrieveSCU queryRetrieveSCU;

        [Inject]
        private IWindowManager _windowManager;

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
                        QueryStudies(selectedPatient);
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
                        QuerySeries(selectedStudy);
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
                        QueryImages(selectedSeries);
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

        public async void Handle(ClientMessageItem message)
        {
            await QueryPatients(message);
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }

        private (string serverIp, int serverPort, string serverAet, string localAet) GetServerConfig()
        {
            var configVm = (Parent as QueryRetrieveViewModel).ServerConfigViewModel;
            return (configVm.ServerIP, configVm.ParseServerPort(), configVm.ServerAET, configVm.LocalAET);
        }

        private async Task QueryPatients(ClientMessageItem message)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));
            IsBusy = true;

            List<DicomDataset> result = null;

            try
            {
                result = await queryRetrieveSCU.QueryPatients(message.ServerIP, message.ServerPort, message.ServerAET, message.LocalAET);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
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

        private async void QueryStudies(IDicomObjectLevel obj)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));
            IsBusy = true;

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            List<DicomDataset> result = null;

            try
            {
                result = await queryRetrieveSCU.QueryStudiesByPatientAsync(serverIp, serverPort, serverAet, localAet, obj.UID);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
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

        private async void QuerySeries(IDicomObjectLevel obj)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));
            IsBusy = true;

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            List<DicomDataset> result = null;

            try
            {
                result = await queryRetrieveSCU.QuerySeriesByStudyAsync(serverIp, serverPort, serverAet, localAet, obj.UID);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
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

        private async void QueryImages(IDicomObjectLevel obj)
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));
            IsBusy = true;

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();

            List<DicomDataset> result = null;
            try
            {
                result = await queryRetrieveSCU.QueryImagesByStudyAndSeriesAsync(
                    serverIp, serverPort, serverAet, localAet, obj.Parent.UID, obj.UID);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
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

        public async void PreviewImage()
        {
            _eventAggregator.Publish(new BusyStateItem(true), nameof(QueryResultViewModel));
            IsBusy = true;

            var (serverIp, serverPort, serverAet, localAet) = GetServerConfig();
            DicomDataset result = null;
            try
            {
                result = await queryRetrieveSCU.GetImagesBySOPInstanceAsync(
                    serverIp, serverPort, serverAet, localAet,
                    selectedImage.Parent.Parent.UID, selectedImage.Parent.UID, selectedImage.UID);
            }
            finally
            {
                IsBusy = false;
                _eventAggregator.Publish(new BusyStateItem(false), nameof(QueryResultViewModel));
            }

            // 有时候查询到的图像没有像素值，无法显示
            if (result != null && result.Contains(DicomTag.PixelData))
            {
                _windowManager.ShowDialog(new PreviewImageViewModel(result));
            }
        }
    }
}

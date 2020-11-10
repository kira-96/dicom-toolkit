using Stylet;
using System.Windows.Shell;

namespace SimpleDICOMToolkit.Services
{
    public class TaskbarService : PropertyChangedBase , ITaskbarService
    {
        private string _description;
        private TaskbarItemProgressState _progressState = TaskbarItemProgressState.None;
        private double _progressValue;

        public string Description
        {
            get => _description;
            set => SetAndNotify(ref _description, value);
        }

        public TaskbarItemProgressState ProgressState
        {
            get => _progressState;
            set => SetAndNotify(ref _progressState, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set => SetAndNotify(ref _progressValue, value);
        }
    }
}

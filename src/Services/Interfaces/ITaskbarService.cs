using System.Windows.Shell;

namespace SimpleDICOMToolkit.Services
{
    public interface ITaskbarService
    {
        /// <summary>
        /// TaskbarItemInfo
        /// Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// TaskbarItemInfo
        /// ProgressState
        /// </summary>
        TaskbarItemProgressState ProgressState { get; set; }

        /// <summary>
        /// TaskbarItemInfo
        /// ProgressValue
        /// </summary>
        double ProgressValue { get; set; }
    }
}

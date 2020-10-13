namespace SimpleDICOMToolkit.ViewModels
{
    public interface IViewModelFactory
    {
        /// <summary>
        /// 获取应用主窗口
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance ViewModel</returns>
        ShellViewModel GetShellViewModel(string key = null);

        /// <summary>
        /// Get WorklistViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        WorklistViewModel GetWorklistViewModel(string key = null);

        /// <summary>
        /// Get WorklistSCPViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        WorklistSCPViewModel GetWorklistSCPViewModel(string key = null);

        /// <summary>
        /// Get PrintViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        PrintViewModel GetPrintViewModel(string key = null);

        /// <summary>
        /// Get StoreViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        StoreViewModel GetStoreViewModel(string key = null);

        /// <summary>
        /// Get StoreSCPViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        StoreSCPViewModel GetStoreSCPViewModel(string key = null);

        /// <summary>
        /// Get PrintSCPViewModel insatnce
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        PrintSCPViewModel GetPrintSCPViewModel(string key = null);

        /// <summary>
        /// Get QueryRetrieveViewModel instance
        /// </summary>
        /// <param name="key"></param>
        /// <returns>instance viewmodel</returns>
        QueryRetrieveViewModel GetQueryRetrieveViewModel(string key = null);

        /// <summary>
        /// get new PreviewImageViewModel instance
        /// </summary>
        /// <returns>new instance viewmodel</returns>
        PreviewImageViewModel GetPreviewImageViewModel();

        /// <summary>
        /// get new EditDicomItemViewModel instance
        /// </summary>
        /// <returns>new instance viewmodel</returns>
        EditDicomItemViewModel GetEditDicomItemViewModel();

        /// <summary>
        /// get new RegistrationViewModel instance
        /// </summary>
        /// <returns>new instance viewmodel</returns>
        RegistrationViewModel GetRegistrationViewModel();

        /// <summary>
        /// get new MoveToViewModel instance
        /// </summary>
        /// <returns>new instance viewmodel</returns>
        MoveToViewModel GetMoveToViewModel();
    }
}

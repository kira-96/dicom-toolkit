namespace SimpleDICOMToolkit.ViewModels
{
    public interface IViewModelFactory
    {
        ShellViewModel GetShellViewModel(string key = null);

        WorklistViewModel GetWorklistViewModel(string key = null);

        WorklistSCPViewModel GetWorklistSCPViewModel(string key = null);

        PrintViewModel GetPrintViewModel(string key = null);

        StoreViewModel GetStoreViewModel(string key = null);

        StoreSCPViewModel GetStoreSCPViewModel(string key = null);

        PrintSCPViewModel GetPrintSCPViewModel(string key = null);

        QueryRetrieveViewModel GetQueryRetrieveViewModel(string key = null);

        PreviewImageViewModel GetPreviewImageViewModel();

        EditDicomItemViewModel GetEditDicomItemViewModel();

        RegistrationViewModel GetRegistrationViewModel();

        MoveToViewModel GetMoveToViewModel();
    }
}

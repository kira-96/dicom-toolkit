namespace SimpleDICOMToolkit.ViewModels
{
    public interface IViewModelFactory
    {
        ShellViewModel GetShellViewModel(string key = null);

        WorklistViewModel GetWorklistViewModel(string key = null);

        WorklistSCPViewModel GetWorklistSCPViewModel(string key = null);

        PrintViewModel GetPrintViewModel(string key = null);

        CStoreViewModel GetCStoreViewModel(string key = null);

        CStoreSCPViewModel GetStoreSCPViewModel(string key = null);

        PrintSCPViewModel GetPrintSCPViewModel(string key = null);

        QueryRetrieveViewModel GetQueryRetrieveViewModel(string key = null);
    }
}

namespace SimpleDICOMToolkit.ViewModels
{
    public interface IViewModelFactory
    {
        ShellViewModel GetShellViewModel(string key = null);

        WorklistViewModel GetWorklistViewModel(string key = null);

        PrintViewModel GetPrintViewModel(string key = null);

        CStoreViewModel GetCStoreViewModel(string key = null);
    }
}

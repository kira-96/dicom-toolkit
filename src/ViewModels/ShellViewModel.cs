namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel(
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            CStoreViewModel cstoreViewModel,
            CStoreSCPViewModel cstoreSCPViewModel,
            PrintViewModel printViewModel,
            PrintSCPViewModel printSCPViewModel)
        {
            DisplayName = "Simple DICOM Toolkit";

            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(queryRetrieveViewModel);
            Items.Add(cstoreViewModel);
            Items.Add(cstoreSCPViewModel);
            Items.Add(printViewModel);
            Items.Add(printSCPViewModel);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            ActiveItem = Items.Count > 0 ? Items[0] : null;
        }
    }
}

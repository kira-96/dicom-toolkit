namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel(
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            CStoreViewModel cstoreViewModel,
            PrintViewModel printViewModel)
        {
            DisplayName = "Simple DICOM Toolkit";

            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(cstoreViewModel);
            Items.Add(printViewModel);
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();

            ActiveItem = Items.Count > 0 ? Items[0] : null;
        }
    }
}

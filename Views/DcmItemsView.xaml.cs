namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Models;
    using ViewModels;

    /// <summary>
    /// DcmItemsView.xaml 的交互逻辑
    /// </summary>
    public partial class DcmItemsView : UserControl
    {
        public DcmItemsView()
        {
            InitializeComponent();
        }

        private void DcmItemMouseDoubleClick(object s, MouseButtonEventArgs e)
        {
            DcmItemsViewModel vm = DataContext as DcmItemsViewModel;
            DcmItem tappedItem = (s as TreeViewItem).DataContext as DcmItem;

            if (tappedItem.TagType != DcmTagType.Tag)
                return;

            if (vm.IsPixelDataItem(tappedItem))
            {
                vm.ShowDcmImage();
            }
            else
            {
                vm.EditDicomItem(tappedItem);
            }
        }
    }
}

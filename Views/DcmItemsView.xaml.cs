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

            if (vm.IsPixelDataItem((s as TreeViewItem).DataContext as DcmItem))
            {
                vm.ShowDcmImage();
            }
        }
    }
}

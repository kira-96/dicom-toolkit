namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Infrastructure;
    using ViewModels;

    /// <summary>
    /// CStoreFileListView.xaml 的交互逻辑
    /// </summary>
    public partial class StoreFileListView : UserControl
    {
        public StoreFileListView()
        {
            InitializeComponent();
        }

        private void CStoreItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as StoreFileListViewModel).PreviewCStoreItem((sender as ListViewItem).DataContext as IStoreItem);
        }
    }
}

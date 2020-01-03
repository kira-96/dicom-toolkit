namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Models;
    using ViewModels;

    /// <summary>
    /// CStoreFileListView.xaml 的交互逻辑
    /// </summary>
    public partial class CStoreFileListView : UserControl
    {
        public CStoreFileListView()
        {
            InitializeComponent();
        }

        private void CStoreItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as CStoreFileListViewModel).PreviewCStoreItem((sender as ListViewItem).DataContext as CStoreItem);
        }
    }
}

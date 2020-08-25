namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels;

    /// <summary>
    /// CStoreReceivedView.xaml 的交互逻辑
    /// </summary>
    public partial class StoreReceivedView : UserControl
    {
        public StoreReceivedView()
        {
            InitializeComponent();
        }

        private void ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as StoreReceivedViewModel).ShowReceivedFile((sender as ListViewItem).Content as string);
        }
    }
}

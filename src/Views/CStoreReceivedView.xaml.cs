namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ViewModels;

    /// <summary>
    /// CStoreReceivedView.xaml 的交互逻辑
    /// </summary>
    public partial class CStoreReceivedView : UserControl
    {
        public CStoreReceivedView()
        {
            InitializeComponent();
        }

        private void ItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (DataContext as CStoreReceivedViewModel).ShowReceivedFile((sender as ListViewItem).Content as string);
        }
    }
}

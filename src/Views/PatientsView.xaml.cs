using System.Windows.Controls;

namespace SimpleDICOMToolkit.Views
{
    /// <summary>
    /// PatientsView.xaml 的交互逻辑
    /// </summary>
    public partial class PatientsView
    {
        public PatientsView()
        {
            InitializeComponent();
        }

        private void ItemMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as ViewModels.PatientsViewModel).ViewDetails((sender as ListViewItem).DataContext as Server.WorklistItem);
        }
    }
}

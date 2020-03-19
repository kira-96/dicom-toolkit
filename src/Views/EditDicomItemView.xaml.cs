namespace SimpleDICOMToolkit.Views
{
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// EditDicomItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditDicomItemView : Window
    {
        public EditDicomItemView()
        {
            InitializeComponent();
        }

        private void OnOK(object s, RoutedEventArgs e)
        {
            (this.DataContext as EditDicomItemViewModel).NotifyUpdateDicomItemValues();
            this.DialogResult = true;
        }

        private void OnCancel(object s, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

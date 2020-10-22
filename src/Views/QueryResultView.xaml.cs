namespace SimpleDICOMToolkit.Views
{
    using System.Windows.Input;
    using ViewModels;

    /// <summary>
    /// QueryResultView.xaml 的交互逻辑
    /// </summary>
    public partial class QueryResultView
    {
        public QueryResultView()
        {
            InitializeComponent();
        }

        private async void ImagesListMouseDoubleClick(object s, MouseButtonEventArgs e)
        {
            await (DataContext as QueryResultViewModel).PreviewImageAsync();
        }
    }
}

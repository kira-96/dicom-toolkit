using System.Windows;
using System.Windows.Input;

namespace SimpleDICOMToolkit.Views
{
    /// <summary>
    /// PreviewImageView.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewImageView
    {
        private bool isFullscreen = false;
        private bool isTopmost = false;
        private Rect normalWindowRect;
        private WindowState normalState = WindowState.Normal;
        private WindowStyle normalStyle = WindowStyle.SingleBorderWindow;
        private ResizeMode normalResizeMode = ResizeMode.CanResize;

        public PreviewImageView()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(WindowCommands.FullscreenCommand, ToggleFullscreen));
        }

        private void ToggleFullscreen(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isFullscreen)
            {
                // backup state
                isTopmost = this.Topmost;
                normalState = this.WindowState;
                normalStyle = this.WindowStyle;
                normalResizeMode = this.ResizeMode;
                normalWindowRect = new Rect(Left, Top, ActualWidth, ActualHeight);

                // enter fullscreen
                this.Topmost = true;
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;
                this.Width = SystemParameters.WorkArea.Width;
                this.Height = SystemParameters.WorkArea.Height;
                isFullscreen = true;
            }
            else
            {
                // exit fullscreen
                this.Topmost = isTopmost;
                this.WindowState = normalState;
                this.WindowStyle = normalStyle;
                this.ResizeMode = normalResizeMode;
                this.Left = normalWindowRect.Left;
                this.Top = normalWindowRect.Top;
                this.Width = normalWindowRect.Width;
                this.Height = normalWindowRect.Height;
                isFullscreen = false;
            }
        }
    }

    public class WindowCommands
    {
        public static RoutedUICommand FullscreenCommand { get; } = new RoutedUICommand();
    }
}

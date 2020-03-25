namespace SimpleDICOMToolkit.Views
{
    using StyletIoC;
    using System.Windows;
    using System.Windows.Forms;
    using Services;

    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        [Inject]
        private INotificationService notificationService;

        [Inject]
        private IDialogServiceEx dialogService;

        private NotifyIcon notifyIcon;

        private System.Windows.Controls.ContextMenu trayIconContextMenu;

        public ShellView()
        {
            InitializeComponent();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            notifyIcon = new NotifyIcon()
            {
                Visible = false,
                Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                Icon = new System.Drawing.Icon(
                    System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SimpleDICOMToolkit.Icons.icon.ico"),
                    System.Windows.Forms.SystemInformation.SmallIconSize)
            };

            notifyIcon.MouseClick += TrayIconMouseClick;
            notifyIcon.MouseDoubleClick += TrayIconMouseDoubleClick;

            trayIconContextMenu = (System.Windows.Controls.ContextMenu)FindResource("TrayIconContextMenu");
        }

        private void Window_Loaded(object s, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
            notificationService.Initialize(notifyIcon);
        }

        private void Window_Closing(object s, System.ComponentModel.CancelEventArgs e)
        {
            // 弹窗提示是否确定退出
            MessageBoxResult result = dialogService.ShowMessageBox(
                "确定要退出吗？", "退出应用？", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Information, MessageBoxResult.No, this);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private void Window_Closed(object s, System.EventArgs e)
        {
            notifyIcon.MouseClick -= TrayIconMouseClick;
            notifyIcon.MouseDoubleClick -= TrayIconMouseDoubleClick;
            notifyIcon.Dispose();
        }

        private void Window_Deactivated(object s, System.EventArgs e)
        {
            trayIconContextMenu.IsOpen = false;
        }

        private void TrayIconMouseClick(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Open the Notify icon context menu
                trayIconContextMenu.IsOpen = true;

                // Required to close the Tray icon when Deactivated is called
                // See: http://copycodetheory.blogspot.be/2012/07/notify-icon-in-wpf-applications.html
                Activate();
            }
        }

        private void TrayIconMouseDoubleClick(object s, MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        private void MenuItemShowClick(object s, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        private void MenuItemExitClick(object s, RoutedEventArgs e)
        {
            Close();
        }
    }
}

namespace SimpleDICOMToolkit.Views
{
    using StyletIoC;
    using System.Windows;
    using System.Windows.Forms;
    using Services;
    using Utils;
    using System;
    using System.Windows.Interop;
    using System.Threading.Tasks;

    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        [Inject]
        private INotificationService notificationService;

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
                Icon = new System.Drawing.Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SimpleDICOMToolkit.Icons.icon.ico"))
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
            MessageBoxResult result = ShowMessageBoxBeforeExit();

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
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
            Activate();
        }

        private void MenuItemShowClick(object s, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Activate();
        }

        private void MenuItemExitClick(object s, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 显示消息框并将鼠标移动到默认按钮位置
        /// </summary>
        /// <returns>MessageBoxResult</returns>
        private MessageBoxResult ShowMessageBoxBeforeExit()
        {
            string title = "退出应用？";
            IntPtr thisWindow = new WindowInteropHelper(this).Handle;
            bool messageBoxClosed = false;

            // 由于MessageBox会阻塞主线程
            // 需要使用另外一个线程进行操作
            Task.Run(() =>
            {
                IntPtr messageBoxHandle = IntPtr.Zero;
                IntPtr activeWindow = IntPtr.Zero;
                // 线程启动时可能MessageBox还没弹出，所以需要循环查找
                while (messageBoxHandle == IntPtr.Zero || activeWindow != messageBoxHandle)
                {
                    Task.Delay(100);
                    if (messageBoxClosed) return;
                    messageBoxHandle = WindowsAPI.FindWindow(null, title);  // 查找对话框窗口
                    activeWindow = WindowsAPI.GetForegroundWindow();  // 查找当前激活窗口，MessageBox会弹出到最前
                }
                WindowsAPI.GetWindowRect(messageBoxHandle, out MyRect rect);
                WindowsAPI.SetCursorPos(rect.Right - 72, rect.Bottom - 36);  // 设置鼠标位置
            });

            MessageBoxResult result = System.Windows.MessageBox.Show(
                this,
                "确定要退出吗？", title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information,
                MessageBoxResult.No);

            messageBoxClosed = true;

            return result;
        }
    }
}

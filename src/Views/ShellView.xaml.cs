namespace SimpleDICOMToolkit.Views
{
    using StyletIoC;
    using Ookii.Dialogs.Wpf;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Interop;
    using System.Windows.Media;
    using ContextMenu = System.Windows.Controls.ContextMenu;
    using MenuItem = System.Windows.Controls.MenuItem;
    using Logging;
    using Services;
    using static Utils.LanguageHelper;
    using static Utils.WindowsAPI;
    using static Utils.SysUtil;

    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        /// <summary>
        /// Menu Item ID
        /// </summary>
        private const uint IDM_ABOUT = 1001;

        [Inject]
        private INotificationService notificationService;

        [Inject(Key = "filelogger")]
        private ILoggerService logger;

        [Inject]
        private IDialogServiceEx dialogService;

        private NotifyIcon notifyIcon;

        private ContextMenu trayIconContextMenu;

        private readonly Dictionary<string, string> supportLanguages = new Dictionary<string, string>()
        {
            { "zh-CN", "简体中文" },
            { "en-US", "English" },
            /* 添加语言 */
        };

        public ShellView()
        {
            InitializeComponent();
            InitializeTrayIcon();
            LoadDefaultLanguage();
            ApplyTheme();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hWnd = new WindowInteropHelper(this).Handle;

            // 修改系统菜单
            ModifySystemMenu(hWnd);

            // 添加窗口消息钩子
            HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(WndProc));
        }

        /// <summary>
        /// 注意，移除系统菜单后，相应功能也会被禁用
        /// 由于本窗口没有调整大小功能，可以只保留“移动”和“关闭”
        /// 并添加“关于”
        /// </summary>
        private void ModifySystemMenu(IntPtr hWnd)
        {
            IntPtr hMenu = GetSystemMenu(hWnd);

            /** 系统菜单默认排列
             * 还原
             * 移动
             * 大小
             * 最小化/还原
             * 最大化
             * 关闭
             */
            //RemoveMenu(hMenu, 0, MF_DISABLED | MF_BYPOSITION);
            //RemoveMenu(hMenu, 1, MF_DISABLED | MF_BYPOSITION);
            //RemoveMenu(hMenu, 1, MF_DISABLED | MF_BYPOSITION);
            //RemoveMenu(hMenu, 1, MF_DISABLED | MF_BYPOSITION);
            InsertMenu(hMenu, 1, MF_SEPARATOR, 0, null);  // 添加分割线
            InsertMenu(hMenu, 8, MF_BYPOSITION, IDM_ABOUT, GetXmlStringByKey("MenuAbout"));
        }

        /// <summary>
        /// 窗口消息处理函数
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // 这里只需要处理“关于”菜单按钮事件
            if (msg == WM_SYSCOMMAND)
            {
                if (wParam.ToInt32() == IDM_ABOUT)
                {
                    handled = true;
                    ShowAbout();
                }
            }
            else if (msg == WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                ApplyTheme();
            }
            else
            {
                // do nothing
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 显示"关于"
        /// </summary>
        private void ShowAbout()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            Version osVersion = Environment.OSVersion.Version;

            string caption = GetXmlStringByKey("AboutCaption");
            string versionInfo = string.Format(GetXmlStringByKey("AboutVersionFormatter"), osVersion, version);

            if (TaskDialog.OSSupportsTaskDialogs)
            {
                using (TaskDialog dialog = new TaskDialog())
                {
                    dialog.AllowDialogCancellation = true;
                    dialog.ExpandedByDefault = true;
                    dialog.EnableHyperlinks = true;
                    dialog.WindowTitle = caption;
                    dialog.MainInstruction = GetXmlStringByKey("AboutInstruction");
                    dialog.Content = GetXmlStringByKey("AboutContent");
                    dialog.ExpandedInformation = versionInfo;
                    dialog.Footer = GetXmlStringByKey("AboutFooter");
                    dialog.FooterIcon = TaskDialogIcon.Information;
                    dialog.HyperlinkClicked += (s, e) => Utils.ProcessUtil.OpenHyperlink(e.Href);

                    dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                    dialog.ShowDialog(this);
                }
            }
            else
            {
                dialogService.ShowMessageBox(
                    versionInfo, caption, 
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, this);
            }

        }

        private void ApplyTheme()
        {
            if (!IsWindowPrevalenceAccentColor())
            {
                return;
            }

            if (IsActive)
            {
                Color accentColor = GetAccentColor();
                ContentGrid.Background = new SolidColorBrush(accentColor);
                TabText.Foreground = new SolidColorBrush(GetReverseForegroundColor(accentColor));
            }
            else
            {
                ContentGrid.Background = new SolidColorBrush(Colors.White);
                TabText.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void InitializeTrayIcon()
        {
            notifyIcon = new NotifyIcon()
            {
                Visible = false,
                Text = Assembly.GetExecutingAssembly().GetName().Name,
                Icon = new System.Drawing.Icon(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("SimpleDICOMToolkit.Icons.icon.ico"),
                    System.Windows.Forms.SystemInformation.SmallIconSize)
            };

            notifyIcon.MouseClick += TrayIconMouseClick;
            notifyIcon.MouseDoubleClick += TrayIconMouseDoubleClick;

            trayIconContextMenu = (ContextMenu)FindResource("TrayIconContextMenu");
        }

        private void LoadDefaultLanguage()
        {
            string code = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            var languageItems = (trayIconContextMenu.Items[1] as MenuItem).Items;

            if (!supportLanguages.ContainsKey(code))
            {
                logger.Warn("Language {0} is not supported, default use zh-CN.", code);
                return;
            }

            foreach (var item in languageItems)
            {
                var menuItem = item as MenuItem;
                if (menuItem.Tag.ToString() == code)
                {
                    if (!menuItem.IsChecked)
                    {
                        menuItem.IsChecked = true;
                        LoadXmlStringResourceByCode(code);
                    }
                }
                else
                {
                    menuItem.IsChecked = false;
                }
            }
        }

        private void Window_Loaded(object s, RoutedEventArgs e)
        {
            notifyIcon.Visible = true;
            notificationService.Initialize(notifyIcon);
        }

        private void Window_Closing(object s, System.ComponentModel.CancelEventArgs e)
        {
            string caption = GetXmlStringByKey("ExitCaption");
            string content = GetXmlStringByKey("ExitContent");

            // 弹窗提示是否确定退出
            MessageBoxResult result = dialogService.ShowMessageBox(
                content, caption, 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Information, MessageBoxResult.No, this);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private void Window_Closed(object s, EventArgs e)
        {
            notifyIcon.MouseClick -= TrayIconMouseClick;
            notifyIcon.MouseDoubleClick -= TrayIconMouseDoubleClick;
            notifyIcon.Dispose();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void Window_Deactivated(object s, EventArgs e)
        {
            trayIconContextMenu.IsOpen = false;
            ApplyTheme();
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

        private void LanguageClick(object s, RoutedEventArgs e)
        {
            var menuItem = s as MenuItem;
            LoadXmlStringResourceByCode(menuItem.Tag.ToString());

            var languageItems = (menuItem.Parent as MenuItem).Items;

            foreach (var item in languageItems)
            {
                (item as MenuItem).IsChecked = false;
            }
            menuItem.IsChecked = true;
        }
    }
}

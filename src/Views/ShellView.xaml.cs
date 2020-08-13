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
    using static Helpers.InteropHelper;

    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        /// <summary>
        /// Menu Item ID
        /// </summary>
        private const uint IDM_ABOUT = 1001;

        private readonly ILoggerService logger;

        private readonly II18nService i18NService;

        private readonly IDialogServiceEx dialogService;

        private readonly INotificationService notificationService;

        private readonly IAppearanceService appearanceService;
        private readonly IUpdateService updateService;
        private NotifyIcon notifyIcon;

        private ContextMenu trayIconContextMenu;

        private readonly Dictionary<string, string> supportLanguages = new Dictionary<string, string>()
        {
            { "zh-CN", "简体中文" },
            { "en-US", "English" },
            /* 添加语言 */
        };

        public ShellView(
            II18nService i18NService,
            IDialogServiceEx dialogService,
            INotificationService notificationService,
            IAppearanceService appearanceService,
            IUpdateService updateService,
            [Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            InitializeComponent();

            this.i18NService = i18NService;
            this.dialogService = dialogService;
            this.notificationService = notificationService;
            this.appearanceService = appearanceService;
            this.updateService = updateService;
            this.logger = loggerService;

            InitializeTrayIcon();
            LoadDefaultLanguage();
            appearanceService.ApplyAccentColor();
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
            appearanceService.WatchWindowsColor(this);
            appearanceService.AccentColorChanged += OnAccentColorChanged;
        }

        /// <summary>
        /// 添加“关于”
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
            InsertMenu(hMenu, 1, MF_SEPARATOR, 0, null);  // 添加分割线
            InsertMenu(hMenu, 8, MF_BYPOSITION, IDM_ABOUT, i18NService.GetXmlStringByKey("MenuAbout"));
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
            if (msg == WM_SYSCOMMAND)
            {
                if (wParam.ToInt32() == IDM_ABOUT)
                {
                    handled = true;
                    ShowAbout();
                }
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

            string caption = i18NService.GetXmlStringByKey("AboutCaption");
            string versionInfo = string.Format(i18NService.GetXmlStringByKey("AboutVersionFormatter"), osVersion, version);

            if (TaskDialog.OSSupportsTaskDialogs)
            {
                using TaskDialog dialog = new TaskDialog
                {
                    AllowDialogCancellation = true,
                    ExpandedByDefault = true,
                    EnableHyperlinks = true,
                    WindowTitle = caption,
                    MainInstruction = i18NService.GetXmlStringByKey("AboutInstruction"),
                    Content = i18NService.GetXmlStringByKey("AboutContent"),
                    ExpandedInformation = versionInfo,
                    Footer = i18NService.GetXmlStringByKey("AboutFooter"),
                    FooterIcon = TaskDialogIcon.Information
                };

                var checkUpdateButton = new TaskDialogButton(i18NService.GetXmlStringByKey("CheckUpdate"));
                dialog.Buttons.Add(checkUpdateButton);
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok) { Default = true });
                dialog.HyperlinkClicked += (s, e) => Helpers.ProcessHelper.OpenHyperlink(e.Href);
                var result = dialog.ShowDialog(this);

                if (result == checkUpdateButton)
                {
                    updateService.CheckForUpdate();
                }
            }
            else
            {
                dialogService.ShowMessageBox(
                    versionInfo, caption, 
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, this);
            }

        }

        private ResourceDictionary CommonResources
        {
            get
            {
                return System.Windows.Application.Current.Resources.MergedDictionaries[2].MergedDictionaries[0];
            }
        }

        private ResourceDictionary ApplicationResources
        {
            get
            {
                return System.Windows.Application.Current.Resources;
            }
        }

        /// <summary>
        /// 设置当前窗口控件的颜色
        /// 由于需要兼容到窗口 激活/非激活
        /// 以及是否应用AccentColor到窗体的选项
        /// 所以比较复杂
        /// * 仅对当前窗体生效
        /// </summary>
        private void ApplyTheme()
        {
            if (IsActive)
            {
                Resources["ButtonBackground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                Resources["ButtonForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentForegroundColor"]);
                Resources["HeaderBackground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                Resources["HeaderForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentForegroundColor"]);

                if (appearanceService.IsWindowPrevalenceAccentColor)
                {
                    Resources["CommonBackground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                    Resources["TabItemBackground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                    Resources["TabItemForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentForegroundColor"]);
                    Resources["SelectedTabItemBackground"] = new SolidColorBrush((Color)ApplicationResources["AccentForegroundColor"]);
                    Resources["SelectedTabItemForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                    Resources["IndicatorColor"] = new SolidColorBrush(Colors.White);
                }
                else
                {
                    Resources["CommonBackground"] = new SolidColorBrush((Color)CommonResources["CommonBackgroundColor"]);
                    Resources["TabItemBackground"] = new SolidColorBrush((Color)CommonResources["CommonBackgroundColor"]);
                    Resources["TabItemForeground"] = new SolidColorBrush((Color)CommonResources["CommonForegroundColor"]);
                    Resources["SelectedTabItemBackground"] = new SolidColorBrush((Color)CommonResources["CommonBackgroundColor"]);
                    Resources["SelectedTabItemForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                    Resources["IndicatorColor"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                }
            }
            else
            {
                Resources["CommonBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveBackgroundColor"]);
                Resources["ButtonBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlBackgroundColor"]);
                Resources["ButtonForeground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlForegroundColor"]);
                Resources["HeaderBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlBackgroundColor"]);
                Resources["HeaderForeground"] = new SolidColorBrush((Color)CommonResources["NonactiveControlForegroundColor"]);
                Resources["TabItemBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveBackgroundColor"]);
                Resources["TabItemForeground"] = new SolidColorBrush((Color)CommonResources["NonactiveForegroundColor"]);
                Resources["SelectedTabItemBackground"] = new SolidColorBrush((Color)CommonResources["NonactiveBackgroundColor"]);
                Resources["SelectedTabItemForeground"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
                Resources["IndicatorColor"] = new SolidColorBrush((Color)ApplicationResources["AccentColor"]);
            }
        }

        private void OnAccentColorChanged(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                appearanceService.ApplyAccentColor();
                ApplyTheme();
            });
        }

        private void WindowPrevalenceAccentColorChanged(object s, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => ApplyTheme());
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

            trayIconContextMenu = (ContextMenu)Resources["TrayIconContextMenu"];
        }

        private void LoadDefaultLanguage()
        {
            string code = i18NService.GetSystemLanguageCode();
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
                        i18NService.ApplyXmlStringsResourceByCode(code);
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
            notificationService.Register(MainToaster);
            appearanceService.StartMonitoringWindowPrevalenceAccentColor();
            appearanceService.WindowPrevalenceAccentColorChanged += WindowPrevalenceAccentColorChanged;
        }

        private void Window_Closing(object s, System.ComponentModel.CancelEventArgs e)
        {
            string caption = i18NService.GetXmlStringByKey("ExitCaption");
            string content = i18NService.GetXmlStringByKey("ExitContent");

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
            appearanceService.AccentColorChanged -= OnAccentColorChanged;
            appearanceService.WindowPrevalenceAccentColorChanged -= WindowPrevalenceAccentColorChanged;
            appearanceService.StopMonitoringWindowPrevalenceAccentColor();

            notifyIcon.MouseClick -= TrayIconMouseClick;
            notifyIcon.MouseDoubleClick -= TrayIconMouseDoubleClick;
            notifyIcon.Dispose();
        }

        private void Window_Activated(object s, EventArgs e)
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
            i18NService.ApplyXmlStringsResourceByCode(menuItem.Tag.ToString());

            var languageItems = (menuItem.Parent as MenuItem).Items;

            foreach (var item in languageItems)
            {
                (item as MenuItem).IsChecked = false;
            }
            menuItem.IsChecked = true;
        }
    }
}

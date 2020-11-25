namespace SimpleDICOMToolkit.Views
{
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
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

            trayIconContextMenu = (ContextMenu)Resources["TrayIconContextMenu"];
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
            string versionInfo = string.Format(i18NService.GetXmlStringByKey("AboutVersionFormatter"),
                Environment.OSVersion.Version, Assembly.GetExecutingAssembly().GetName().Version);

            MessageBox.Show(i18NService.GetXmlStringByKey("AboutContent") + "\n\n" + versionInfo, i18NService.GetXmlStringByKey("AboutCaption"));
        }

        private ResourceDictionary ApplicationResources => Application.Current.Resources;
        private ResourceDictionary CommonResources => Application.Current.Resources.MergedDictionaries[2].MergedDictionaries[0];

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
                    Resources["SelectedTabItemBackground"] = new SolidColorBrush(Colors.White);
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                appearanceService.ApplyAccentColor();
                ApplyTheme();
            });
        }

        private void WindowPrevalenceAccentColorChanged(object s, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ApplyTheme());
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
            notificationService.RegistNotify(TrayIcon);
            notificationService.RegistToast(MainToaster);
            appearanceService.StartMonitoringWindowPrevalenceAccentColor();
            appearanceService.WindowPrevalenceAccentColorChanged += WindowPrevalenceAccentColorChanged;

            LoadDefaultLanguage();
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

        private async void MenuItemCheckUpdateClick(object s, RoutedEventArgs e)
        {
            await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("ToastCheckingUpdate"), new TimeSpan(0, 0, 2));
            await updateService.CheckForUpdateAsync();
        }
    }
}

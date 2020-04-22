using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;

namespace Config.Views
{
    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView
    {
        public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        public ShellView()
        {
            InitializeComponent();
            ApplyTheme();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hWnd = new WindowInteropHelper(this).Handle;

            // 添加窗口消息钩子
            HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(WndProc));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ApplyTheme();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            ApplyTheme();
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
            if (msg == WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                ApplyTheme();
            }

            return IntPtr.Zero;
        }

        private void ApplyTheme()
        {
            if (!IsWindowPrevalenceAccentColor())
            {
                return;
            }

            if (IsActive)
            {
                var accentBrush = new SolidColorBrush(GetAccentColor());
                ContentGrid.Background = accentBrush;
                OkButton.Background = accentBrush;
            }
            else
            {
                var inactiveColor = Colors.White;
                ContentGrid.Background = new SolidColorBrush(inactiveColor);
                OkButton.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private Color GetAccentColor()
        {
            using (RegistryKey dwm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", false))
            {
                if (dwm.GetValueNames().Contains("AccentColor"))
                {
                    int accentColor = (int)dwm.GetValue("AccentColor");
                    return Color.FromArgb(
                        (byte)((accentColor >> 24) & 0xFF),
                        (byte)(accentColor & 0xFF),
                        (byte)((accentColor >> 8) & 0xFF),
                        (byte)((accentColor >> 16) & 0xFF));
                }
            }

            return SystemParameters.WindowGlassColor;
        }

        public bool IsWindowPrevalenceAccentColor()
        {
            using (RegistryKey dwm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", false))
            {
                if (dwm.GetValueNames().Contains("ColorPrevalence"))
                {
                    int colorPrevalence = (int)dwm.GetValue("ColorPrevalence");

                    return colorPrevalence == 1;
                }
            }

            return false;
        }
    }
}

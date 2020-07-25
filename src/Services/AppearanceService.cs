using StyletIoC;
using System;
using System.Management;
using System.Security.Principal;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SimpleDICOMToolkit.Logging;
using SimpleDICOMToolkit.Helpers;
using static SimpleDICOMToolkit.Helpers.SystemHelper;

namespace SimpleDICOMToolkit.Services
{
    public class AppearanceService : IAppearanceService
    {
        private readonly ILoggerService Logger;

        private ManagementEventWatcher systemUsesLightThemeWatcher;
        private ManagementEventWatcher windowPrevalenceAccentColorWatcher;

        public bool IsSystemUsesLightTheme => SystemUsesLightTheme();

        public bool IsWindowPrevalenceAccentColor => IsWindowPrevalenceAccentColor();

        public event EventHandler SystemUsesLightThemeChanged = delegate { };
        public event EventHandler WindowPrevalenceAccentColorChanged = delegate { };
        public event EventHandler AccentColorChanged = delegate { };

        public AppearanceService([Inject("filelogger")] ILoggerService loggerService)
        {
            Logger = loggerService;
        }

        public void StartMonitoringSystemUsesLightTheme()
        {
            try
            {
                var currentUser = WindowsIdentity.GetCurrent();
                if (currentUser != null && currentUser.User != null)
                {
                    var wqlEventQuery = new EventQuery(string.Format(@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize' AND ValueName='SystemUsesLightTheme'", currentUser.User.Value));
                    systemUsesLightThemeWatcher = new ManagementEventWatcher(wqlEventQuery);
                    systemUsesLightThemeWatcher.EventArrived += AppsUseLightThemeWatcher_EventArrived;
                    systemUsesLightThemeWatcher.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not start monitoring system uses light theme. Exception: {0}", ex.Message);
            }
        }

        public void StartMonitoringWindowPrevalenceAccentColor()
        {
            try
            {
                var currentUser = WindowsIdentity.GetCurrent();
                if (currentUser != null && currentUser.User != null)
                {
                    var wqlEventQuery = new EventQuery(string.Format(@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\SOFTWARE\\Microsoft\\Windows\\DWM' AND ValueName='ColorPrevalence'", currentUser.User.Value));
                    windowPrevalenceAccentColorWatcher = new ManagementEventWatcher(wqlEventQuery);
                    windowPrevalenceAccentColorWatcher.EventArrived += WindowPrevalenceAccentColorWatcher_EventArrived;
                    windowPrevalenceAccentColorWatcher.Start();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not start monitoring window prevalence accent color. Exception: {0}", e.Message);
            }
        }

        public void StopMonitoringSystemUsesLightTheme()
        {
            try
            {
                if (systemUsesLightThemeWatcher != null)
                {
                    systemUsesLightThemeWatcher.Stop();
                    systemUsesLightThemeWatcher.EventArrived -= AppsUseLightThemeWatcher_EventArrived;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not stop monitoring system uses light theme. Exception: {0}", ex.Message);
            }
        }

        public void StopMonitoringWindowPrevalenceAccentColor()
        {
            try
            {
                if (windowPrevalenceAccentColorWatcher != null)
                {
                    windowPrevalenceAccentColorWatcher.Stop();
                    windowPrevalenceAccentColorWatcher.EventArrived -= WindowPrevalenceAccentColorWatcher_EventArrived;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not stop monitoring window prevalence accent color. Exception: {0}", e.Message);
            }
        }

        public void WatchWindowsColor(object window)
        {
            if (window is Window win)
            {
                IntPtr hWnd = new WindowInteropHelper(win).Handle;
                HwndSource.FromHwnd(hWnd).AddHook(new HwndSourceHook(WndProc));
            }
        }

        public void ApplyAccentColor()
        {
            Color accentColor = !IsWindows7OrLower ? GetAccentColor() : Color.FromRgb(0x00, 0x78, 0xd7);
            Color accentForeground = GetReverseForegroundColor(accentColor);
            Application.Current.Resources["AccentColor"] = accentColor;
            Application.Current.Resources["AccentForegroundColor"] = accentForeground;
            Application.Current.Resources["AccentBrush"] = new SolidColorBrush(accentColor);
        }

        private void AppsUseLightThemeWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            SystemUsesLightThemeChanged(this, new EventArgs());
        }

        private void WindowPrevalenceAccentColorWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            WindowPrevalenceAccentColorChanged(this, new EventArgs());
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == InteropHelper.WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                AccentColorChanged(this, new EventArgs());
            }

            return IntPtr.Zero;
        }
    }
}

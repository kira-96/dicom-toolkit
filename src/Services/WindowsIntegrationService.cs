using System;
using System.Linq;
using System.Management;
using System.Security.Principal;
using Microsoft.Win32;
using SimpleDICOMToolkit.Logging;

namespace SimpleDICOMToolkit.Services
{
    public class WindowsIntegrationService : IWindowsIntegrationService
    {
        private ILoggerService Logger => SimpleIoC.Get<ILoggerService>("filelogger");

        private ManagementEventWatcher systemUsesLightThemeWatcher;
        private ManagementEventWatcher windowPrevalenceAccentColorWatcher;

        public bool IsSystemUsingLightTheme
        {
            get
            {
                int registrySystemUsesLightTheme = 0;

                try
                {
                    registrySystemUsesLightTheme = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", 0);
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not get system uses light theme from registry. Exception: {0}", ex.Message);
                }

                return registrySystemUsesLightTheme == 1;
            }
        }

        public bool IsWindowPrevalenceAccentColor
        {
            get
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

        public event EventHandler SystemUsesLightThemeChanged = delegate { };
        public event EventHandler WindowPrevalenceAccentColorChanged = delegate { };

        public void StartMonitoringSystemUsesLightTheme()
        {
            try
            {
                var currentUser = WindowsIdentity.GetCurrent();
                if (currentUser != null && currentUser.User != null)
                {
                    var wqlEventQuery = new EventQuery(string.Format(@"SELECT * FROM RegistryValueChangeEvent WHERE Hive='HKEY_USERS' AND KeyPath='{0}\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize' AND ValueName='SystemUsesLightTheme'", currentUser.User.Value));
                    this.systemUsesLightThemeWatcher = new ManagementEventWatcher(wqlEventQuery);
                    this.systemUsesLightThemeWatcher.EventArrived += this.AppsUseLightThemeWatcher_EventArrived;
                    this.systemUsesLightThemeWatcher.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not start monitoring system uses light theme. Exception: {0}", ex.Message);
            }
        }

        public void StopMonitoringSystemUsesLightTheme()
        {
            try
            {
                if (this.systemUsesLightThemeWatcher != null)
                {
                    this.systemUsesLightThemeWatcher.Stop();
                    this.systemUsesLightThemeWatcher.EventArrived -= this.AppsUseLightThemeWatcher_EventArrived;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not stop monitoring system uses light theme. Exception: {0}", ex.Message);
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

        private void AppsUseLightThemeWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            this.SystemUsesLightThemeChanged(this, new EventArgs());
        }

        private void WindowPrevalenceAccentColorWatcher_EventArrived(object s, EventArrivedEventArgs e)
        {
            WindowPrevalenceAccentColorChanged(this, new EventArgs());
        }
    }
}

using Microsoft.Win32;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Utils
{
    public static class EnvUtil
    {
        public static string LocalIPAddress
        {
            get
            {
                foreach (IPAddress item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return item.ToString();
                    }
                }

                return "localhost";
            }
        }

        /// <summary>
        /// Gets if the Operating System is Windows 10
        /// </summary>
        /// <returns>True if Windows 10</returns>
        public static bool IsWindows10
        {
            get
            {
                // IMPORTANT: Windows 8.1. and Windows 10 will ONLY admit their real version if your program's manifest 
                // claims to be compatible. Otherwise they claim to be Windows 8. See the first comment on:
                // https://msdn.microsoft.com/en-us/library/windows/desktop/ms724833%28v=vs.85%29.aspx

                // Get Operating system information
                OperatingSystem os = Environment.OSVersion;

                // Get the Operating system version information
                Version vi = os.Version;

                // Pre-NT versions of Windows are PlatformID.Win32Windows. We're not interested in those.

                if (os.Platform == PlatformID.Win32NT)
                {
                    if (vi.Major == 10)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static bool IsWindows7OrLower
        {
            get
            {
                Version v = Environment.OSVersion.Version;

                int versionMajor = v.Major;
                int versionMinor = v.Minor;
                double version = versionMajor + (double)versionMinor / 10;
                return version <= 6.1;
            }
        }

        public static Color GetAccentColor()
        {
            using (RegistryKey dwm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", false))
            {
                if (dwm.GetValueNames().Contains("AccentColor"))
                {
                    int accentColor = (int)dwm.GetValue("AccentColor");
                    // 注意：读取到的颜色为 AABBGGRR
                    return Color.FromArgb(
                        (byte)((accentColor >> 24) & 0xFF),
                        (byte)(accentColor & 0xFF),
                        (byte)((accentColor >> 8) & 0xFF),
                        (byte)((accentColor >> 16) & 0xFF));
                }
            }

            return SystemParameters.WindowGlassColor;
        }

        public static bool AppsUseLightTheme()
        {
            using (RegistryKey personalize = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false))
            {
                if (personalize.GetValueNames().Contains("AppsUseLightTheme"))
                {
                    return (int)personalize.GetValue("AppsUseLightTheme") == 1;
                }
            }

            return true;
        }
    }
}

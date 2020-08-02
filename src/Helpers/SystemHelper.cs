using Microsoft.Win32;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Helpers
{
    public static class SystemHelper
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

        public static bool IsNetworkConnected
        {
            get
            {
                return InteropHelper.InternetGetConnectedState(out int flag, 0);
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

        public static Version RtlNtVersion
        {
            get
            {
                int major = 0, minor = 0, buildNumber = 0;

                InteropHelper.RtlGetNtVersionNumbers(ref major, ref minor, ref buildNumber);

                return new Version(major, minor, buildNumber);
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

        public static Color GetColorizationColor()
        {
            using (RegistryKey dwm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", false))
            {
                if (dwm.GetValueNames().Contains("ColorizationColor"))
                {
                    int accentColor = (int)dwm.GetValue("ColorizationColor");
                    // 注意：读取到的颜色为 AARRGGBB
                    return Color.FromArgb(
                        (byte)((accentColor >> 24) & 0xFF),
                        (byte)((accentColor >> 16) & 0xFF),
                        (byte)((accentColor >> 8) & 0xFF),
                        (byte)(accentColor & 0xFF));
                }
            }

            return SystemParameters.WindowGlassColor;
        }

        public static Color GetAccentColorFromColorizationColor()
        {
            using (RegistryKey dwm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", false))
            {
                if (dwm.GetValueNames().Contains("ColorizationColor"))
                {
                    int accentColor = (int)dwm.GetValue("ColorizationColor");
                    // 注意：读取到的颜色为 AARRGGBB
                    return Color.FromRgb(
                        (byte)((accentColor >> 16) & 0xFF),
                        (byte)((accentColor >> 8) & 0xFF),
                        (byte)(accentColor & 0xFF));
                }
            }

            return SystemParameters.WindowGlassColor;
        }

        /// <summary>
        /// 根据背景色计算前景色(白/黑)
        /// https://github.com/loilo/windows-titlebar-color/blob/master/WindowsAccentColors.js#L53
        /// </summary>
        /// <param name="background">背景颜色</param>
        /// <returns>前景颜色(白/黑)</returns>
        [Obsolete("Use GetReverseForegroundColor instead.")]
        public static Color GetForegroundColor(Color background)
        {
            return (background.R * 2 + background.G * 5 + background.B) <= 1024 /* 8 * 128 */
                ? Colors.White : Colors.Black;
        }

        /// <summary>
        /// 计算能在任何背景色上清晰显示的前景色
        /// https://www.cnblogs.com/walterlv/p/10236517.html
        /// </summary>
        /// <param name="background">背景颜色</param>
        /// <returns>前景颜色(黑/白)</returns>
        public static Color GetReverseForegroundColor(Color background)
        {
            double grayLevel = (0.299 * background.R + 0.587 * background.G + 0.114 * background.B) / 255;

            return grayLevel > 0.5 ? Colors.Black : Colors.White;
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

        public static bool SystemUsesLightTheme()
        {
            using (RegistryKey personalize = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false))
            {
                if (personalize.GetValueNames().Contains("SystemUsesLightTheme"))
                {
                    return (int)personalize.GetValue("SystemUsesLightTheme") == 1;
                }
            }

            return true;
        }

        public static bool IsWindowPrevalenceAccentColor()
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

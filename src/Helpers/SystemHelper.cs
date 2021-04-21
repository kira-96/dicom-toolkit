using Microsoft.Win32;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;

namespace SimpleDICOMToolkit.Helpers
{
    public static class SystemHelper
    {
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        [Obsolete("Use GetLocalIPAddress()")]
        public static string LocalIPAddress
        {
            get
            {
                foreach (IPAddress item in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return item.ToString();
                    }
                }

                return "localhost";
            }
        }

        /// <summary>
        /// 获取本机IP地址
        /// https://stackoverflow.com/questions/6803073/get-local-ip-address
        /// </summary>
        public static string GetLocalIPAddress()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Unspecified);

            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;

            return endPoint.Address.ToString();
        }

        /// <summary>
        /// 当前是否有网络连接
        /// </summary>
        public static bool IsNetworkConnected
        {
            get
            {
                return InteropHelper.InternetGetConnectedState(out _, 0);
            }
        }

        /// <summary>
        /// Gets if the Operating System is Windows 10
        /// </summary>
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

        /// <summary>
        /// Get if the Operating System is Windows 7 or lower
        /// </summary>
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

        [Obsolete]
        public static Version RtlNtVersion
        {
            get
            {
                int major = 0, minor = 0, buildNumber = 0;

                InteropHelper.RtlGetNtVersionNumbers(ref major, ref minor, ref buildNumber);

                return new Version(major, minor, buildNumber);
            }
        }

        /// <summary>
        /// Get current system Accent Color
        /// Windows 8 or later
        /// </summary>
        /// <returns>Accent Color</returns>
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

        /// <summary>
        /// Get current system Colorization Color
        /// Windows 8 or later
        /// </summary>
        /// <returns>Colorization Color</returns>
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

        /// <summary>
        /// Convert Colorization Color to Accent Color
        /// Windows 8 or later
        /// </summary>
        /// <returns>Accent Color</returns>
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

        /// <summary>
        /// 当前系统应用是否为亮色主题（Windows 10）
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 当前系统是否为亮色主题（Windows 10）
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 窗口色是否跟随系统主题颜色（Windows 10）
        /// </summary>
        /// <returns></returns>
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

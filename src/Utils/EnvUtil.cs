using System;
using System.Net;

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
    }
}

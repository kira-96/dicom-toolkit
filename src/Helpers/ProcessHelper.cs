using System.Diagnostics;
using System.IO;

namespace SimpleDICOMToolkit.Helpers
{
    public static class ProcessHelper
    {
        public static void Explore(string path)
        {
            if (File.Exists(path))
            {
                Process.Start("explorer", "/select," + path);
                return;
            }

            if (Directory.Exists(path))
            {
                Process.Start("explorer", path);
                return;
            }

            Process.Start("explorer", "/e");
        }

        public static bool StartProcess(string fileName, string args = null)
        {
            ProcessStartInfo info = new ProcessStartInfo(fileName, args);
            Process process = new Process()
            {
                StartInfo = info
            };

            return process.Start();
        }

        public static void OpenHyperlink(string href)
        {
            Process.Start(href);
        }
    }
}

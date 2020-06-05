using System.Diagnostics;
using System.IO;

namespace SimpleDICOMToolkit.Utils
{
    public static class ProcessUtil
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

        public static void OpenHyperlink(string href)
        {
            Process.Start(href);
        }
    }
}

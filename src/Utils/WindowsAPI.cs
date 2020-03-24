using System;
using System.Runtime.InteropServices;

namespace SimpleDICOMToolkit.Utils
{
    public struct MyRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static class WindowsAPI
    {
        #region System API
        [DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /*
         * ShowWindow() Commands
         */
        //#define SW_HIDE             0
        //#define SW_SHOWNORMAL       1
        //#define SW_NORMAL           1
        //#define SW_SHOWMINIMIZED    2
        //#define SW_SHOWMAXIMIZED    3
        //#define SW_MAXIMIZE         3
        //#define SW_SHOWNOACTIVATE   4
        //#define SW_SHOW             5
        //#define SW_MINIMIZE         6
        //#define SW_SHOWMINNOACTIVE  7
        //#define SW_SHOWNA           8
        //#define SW_RESTORE          9
        //#define SW_SHOWDEFAULT      10
        //#define SW_FORCEMINIMIZE    11
        //#define SW_MAX              11
        public const int SW_NORMAL = 1;
        public const int SW_RESTORE = 9;

        /// <summary>
        /// 将窗口还原,可从最小化还原
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string classname, string windowname);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string classname, string windowname);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern void MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaient);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hwnd, out MyRect rect);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public static extern int SetCursorPos(int x, int y);

        #endregion

        public static void FindWindowAndActive(string classname, string windowname)
        {
            IntPtr hWnd = FindWindow(classname, windowname);
            ShowWindow(hWnd, SW_NORMAL);
            SetForegroundWindow(hWnd);
        }
    }
}

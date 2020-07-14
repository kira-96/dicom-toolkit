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

        public const uint WM_COMMAND = 0x0111;
        public const uint WM_SYSCOMMAND = 0x0112;  // 系统菜单事件消息
        public const uint WM_INITMENUPOPUP = 0x0117;
        public const uint WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;  // 主题色变更消息

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

        public delegate bool WndEnumProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "EnumThreadWindows")]
        public static extern bool EnumThreadWindows(uint threadId, WndEnumProc lpfn, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern void MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaient);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hwnd, out MyRect rect);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public static extern int SetCursorPos(int x, int y);

        public const int MF_INSERT = 0;
        public const int MF_BYCOMMAND = 0;
        public const int MF_BYPOSITION = 0x400;
        public const int MF_SEPARATOR = 0x800;

        public const int MF_ENABLED = 0x00000000;
        public const int MF_GRAYED = 0x00000001;
        public const int MF_DISABLED = 0x00000002;

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert = false);

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", EntryPoint = "GetSubMenu")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", EntryPoint = "InsertMenu")]
        public static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, ulong uIDNewItem, string newItem);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint nPosition, uint nFlags);

        [DllImport("user32.dll", EntryPoint = "DeleteMenu")]
        public static extern bool DeleteMenu(IntPtr hMenu, uint nPosition, uint nFlags);

        [DllImport("user32.dll", EntryPoint = "AppendMenu")]
        public static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string newItem);

        /*
         * Window field offsets for GetWindowLong()
         */
        public const int GWL_WNDPROC = -4;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_USERDATA = -21;
        public const int GWL_ID = -12;

        public const int WS_SYSMENU = 0x00080000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "DefWindowProc")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWmd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern IntPtr PostMessage(IntPtr hWmd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public const int SM_CYCAPTION = 4;
        public const int SM_CXICON = 11;
        public const int SM_CYICON = 12;
        public const int SM_CYSIZE = 31;
        public const int SM_CXFRAME = 32;
        public const int SM_CYFRAME = 33;

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("ntdll.dll", EntryPoint = "RtlGetNtVersionNumbers")]
        public static extern void RtlGetNtVersionNumbers(ref int major, ref int minor, ref int buildNumber);

        #endregion

        public static void FindWindowAndActive(string classname, string windowname)
        {
            IntPtr hWnd = FindWindow(classname, windowname);
            ShowWindow(hWnd, SW_NORMAL);
            SetForegroundWindow(hWnd);
        }

        public static void SetWindowSystemMenu(IntPtr hWnd, bool isEnabled)
        {
            if (Environment.Is64BitProcess)
                SetWindowSystemMenu64(hWnd, isEnabled);
            else
                SetWindowSystemMenu32(hWnd, isEnabled);
        }

        private static void SetWindowSystemMenu32(IntPtr hWnd, bool isEnabled)
        {
            int windowLong = GetWindowLong(hWnd, GWL_STYLE);

            if (isEnabled) windowLong |= WS_SYSMENU;
            else windowLong &= ~WS_SYSMENU;

            SetWindowLong(hWnd, GWL_STYLE, windowLong);
        }

        private static void SetWindowSystemMenu64(IntPtr hWnd, bool isEnabled)
        {
            int windowLong = GetWindowLongPtr(hWnd, GWL_STYLE).ToInt32();


            if (isEnabled) windowLong |= WS_SYSMENU;
            else windowLong &= ~WS_SYSMENU;

            SetWindowLongPtr(hWnd, GWL_STYLE, new IntPtr(windowLong));
        }
    }
}

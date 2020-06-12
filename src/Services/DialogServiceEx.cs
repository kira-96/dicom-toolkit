using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using SimpleDICOMToolkit.Utils;

namespace SimpleDICOMToolkit.Services
{
    public class DialogServiceEx : IDialogServiceEx
    {
        public MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, Window owner = null)
        {
            bool messageBoxClosed = false;
            // 由于MessageBox会阻塞主线程
            // 需要使用另外一个线程进行操作
            Task.Run(() =>
            {
                IntPtr messageBoxHandle = IntPtr.Zero;
                IntPtr activeWindow = IntPtr.Zero;
                // 线程启动时可能MessageBox还没弹出，所以需要循环查找
                while (messageBoxHandle == IntPtr.Zero || activeWindow != messageBoxHandle)
                {
                    Task.Delay(100);
                    if (messageBoxClosed) return;
                    // 暂时只能通过名称查找窗口
                    messageBoxHandle = WindowsAPI.FindWindow(null, caption);  // 查找对话框窗口
                    activeWindow = WindowsAPI.GetForegroundWindow();  // 查找当前激活窗口，MessageBox会弹出到最前
                }

                IntPtr defaultButtonHandle = IntPtr.Zero;
                string buttonClassName = "Button";

                switch (button)
                {
                    case MessageBoxButton.OK:
                        defaultButtonHandle = WindowsAPI.FindWindowEx(messageBoxHandle, IntPtr.Zero, buttonClassName, null);
                        break;
                    case MessageBoxButton.OKCancel:
                        {
                            IntPtr okHandle = WindowsAPI.FindWindowEx(messageBoxHandle, IntPtr.Zero, buttonClassName, null);
                            if (defaultResult == MessageBoxResult.Cancel) defaultButtonHandle = WindowsAPI.FindWindowEx(messageBoxHandle, okHandle, buttonClassName, null);
                            else defaultButtonHandle = okHandle;
                        }
                        break;
                    case MessageBoxButton.YesNoCancel:
                        {
                            IntPtr yesHandle = WindowsAPI.FindWindowEx(messageBoxHandle, IntPtr.Zero, buttonClassName, null);
                            IntPtr noHandle = WindowsAPI.FindWindowEx(messageBoxHandle, yesHandle, buttonClassName, null);
                            if (defaultResult == MessageBoxResult.Cancel) defaultButtonHandle = WindowsAPI.FindWindowEx(messageBoxHandle, noHandle, buttonClassName, null);
                            else if (defaultResult == MessageBoxResult.No) defaultButtonHandle = noHandle;
                            else defaultButtonHandle = yesHandle;
                        }
                        break;
                    case MessageBoxButton.YesNo:
                        {
                            IntPtr yesHandle = WindowsAPI.FindWindowEx(messageBoxHandle, IntPtr.Zero, buttonClassName, null);
                            if (defaultResult == MessageBoxResult.No) defaultButtonHandle = WindowsAPI.FindWindowEx(messageBoxHandle, yesHandle, buttonClassName, null);
                            else defaultButtonHandle = yesHandle;
                        }
                        break;
                    default:
                        break;
                }

                WindowsAPI.GetWindowRect(defaultButtonHandle, out MyRect rect);  // 获取按钮所在矩形
                WindowsAPI.SetCursorPos((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);  // 设置鼠标位置（按钮中心）
            });

            MessageBoxResult result = owner == null ?
                MessageBox.Show(content, caption, button, icon, defaultResult) :
                MessageBox.Show(owner, content, caption, button, icon, defaultResult);

            messageBoxClosed = true;

            return result;
        }

        public bool? ShowOpenFileDialog(string filter = null, bool multiselect = false, Window owner = null, Action<bool?, string[]> callback = null)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = filter ?? "All Files (*.*)|*.*",
                Multiselect = multiselect
            };

            bool? result = owner == null ? dlg.ShowDialog() : dlg.ShowDialog(owner);

            callback?.Invoke(result == true, dlg.FileNames);

            return result;
        }

        public bool? ShowSaveFileDialog(string filter = null, Window owner = null, Action<bool?, string> callback = null)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = filter ?? "All Files (*.*)|*.*"
            };

            bool? result = owner == null ? dlg.ShowDialog() : dlg.ShowDialog(owner);

            callback?.Invoke(result == true, dlg.FileName);

            return result;
        }
    }
}

using System;
using System.Windows;

namespace SimpleDICOMToolkit.Services
{
    public interface IDialogServiceEx
    {
        bool? ShowOpenFileDialog(string filter = null, bool multiselect = false, Window owner = null, Action<bool?, string[]> callback = null);

        bool? ShowSaveFileDialog(string filter = null, Window owner = null, Action<bool?, string> callback = null);

        /// <summary>
        /// 显示MessageBox并将鼠标移动至默认按钮上
        /// </summary>
        /// <param name="content">messageBoxText</param>
        /// <param name="caption">caption</param>
        /// <param name="button">message box button</param>
        /// <param name="icon">message box image</param>
        /// <param name="defaultResult">default button</param>
        /// <param name="owner">message box parent</param>
        /// <returns>message box result</returns>
        MessageBoxResult ShowMessageBox(string content, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, Window owner = null);
    }
}

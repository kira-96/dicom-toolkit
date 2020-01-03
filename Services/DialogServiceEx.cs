using Microsoft.Win32;
using System;
using System.Windows;

namespace SimpleDICOMToolkit.Services
{
    public class DialogServiceEx : IDialogServiceEx
    {
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

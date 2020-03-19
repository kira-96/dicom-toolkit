using System;
using System.Windows;

namespace SimpleDICOMToolkit.Services
{
    public interface IDialogServiceEx
    {
        bool? ShowOpenFileDialog(string filter = null, bool multiselect = false, Window owner = null, Action<bool?, string[]> callback = null);

        bool? ShowSaveFileDialog(string filter = null, Window owner = null, Action<bool?, string> callback = null);
    }
}

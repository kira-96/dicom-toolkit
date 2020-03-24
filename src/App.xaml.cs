namespace SimpleDICOMToolkit
{
    using System;
    using System.Threading;
    using System.Windows;
    using Utils;

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    Mutex mutex = new Mutex(true, "%SimpleDICOMtoolkit%{03804718-95A8-4276-BCE9-76C7B6FE706E}", out bool createNew);

        //    if (createNew)
        //    {
        //        base.OnStartup(e);
        //    }
        //    else
        //    {
        //        MessageBox.Show(
        //            "程序已在运行中。", "提示",
        //            MessageBoxButton.OK, MessageBoxImage.Information);
        //        SetInstanceWindowActive();
        //        Application.Current.Shutdown(1);
        //    }
        //}

        //private void SetInstanceWindowActive()
        //{
        //    IntPtr hInstance = WindowsAPI.FindWindow(null, "Simple DICOM Toolkit");
        //    WindowsAPI.ShowWindow(hInstance, WindowsAPI.SW_NORMAL);
        //    WindowsAPI.SetForegroundWindow(hInstance);
        //}
    }
}

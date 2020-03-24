namespace SimpleDICOMToolkit
{
    using Stylet;
    using StyletIoC;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Client;
    using Logging;
    using Services;
    using Utils;
    using ViewModels;

    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        /// <summary>
        /// 进程互斥
        /// 必须定义在类内部，一旦被释放就无效了
        /// </summary>
        private Mutex mutex;

        /// <summary>
        /// 互斥量名称
        /// </summary>
        private const string MUTEX_NAME = "%SimpleDICOMtoolkit%{03804718-95A8-4276-BCE9-76C7B6FE706E}";

        protected override void OnStart()
        {
            /**
             * 应用程序单例模式
             * 在客户端模式下不检测单例模式，可以启动任意数目的客户端
             */
            // #################################################################################

            if (Args == null || Args.Length == 0 ||
                !CommandLineArgsUtil.IsClientModeOnly(Args[0]))
            {
                mutex = new Mutex(true, MUTEX_NAME, out bool createNew);
                if (!createNew)
                {
                    MessageBox.Show("程序已在运行中。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowsAPI.FindWindowAndActive(null, ShellViewModel.WindowName);  // 激活已经存在的实例窗口

                    // 退出当前应用程序
                    // 尽量不要使用
                    // Application.Shutdown();
                    // 因为在这里使用会触发主窗口的Closing事件
                    System.Environment.Exit(0);
                }
            }

            // #################################################################################

            base.OnStart();

            Dicom.Imaging.ImageManager.SetImplementation(Dicom.Imaging.WPFImageManager.Instance);
            Dicom.Log.LogManager.SetImplementation(Dicom.Log.NLogManager.Instance);
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<ILoggerService>().To<LoggerService>().InSingletonScope().AsWeakBinding();
            builder.Bind<IDialogServiceEx>().To<DialogServiceEx>().InSingletonScope().AsWeakBinding();
            builder.Bind<INotificationService>().To<NotificationService>().InSingletonScope().AsWeakBinding();
            builder.Bind<ICEchoSCU>().To<CEchoSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<ICStoreSCU>().To<CStoreSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IPrintSCU>().To<PrintSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IWorklistSCU>().To<WorklistSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IQueryRetrieveSCU>().To<QueryRetrieveSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IViewModelFactory>().ToAbstractFactory();
        }

        protected override void Configure()
        {
            base.Configure();

            SimpleIoC.GetInstance = this.Container.Get;
            SimpleIoC.GetAllInstances = this.Container.GetAll;
            SimpleIoC.BuildUp = this.Container.BuildUp;
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            Container.Get<ILoggerService>("filelogger").Error(e.Exception);
            Container.Get<IWindowManager>().ShowMessageBox(e.Exception.Message, "出错了!");
        }
    }
}

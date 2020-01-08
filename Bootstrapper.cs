namespace SimpleDICOMToolkit
{
    using Stylet;
    using StyletIoC;
    using System.Windows.Threading;
    using Client;
    using Logging;
    using Services;
    using ViewModels;

    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void OnStart()
        {
            base.OnStart();

            Dicom.Imaging.ImageManager.SetImplementation(Dicom.Imaging.WPFImageManager.Instance);
            Dicom.Log.LogManager.SetImplementation(NLogManager.Instance);
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<ILoggerService>().To<LoggerService>().InSingletonScope().AsWeakBinding();
            builder.Bind<IDialogServiceEx>().To<DialogServiceEx>().InSingletonScope().AsWeakBinding();
            builder.Bind<ICEchoSCU>().To<CEchoSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<ICMoveSCU>().To<CMoveSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<ICStoreSCU>().To<CStoreSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IPrintSCU>().To<PrintSCU>().InSingletonScope().AsWeakBinding();
            builder.Bind<IWorklistSCU>().To<WorklistSCU>().InSingletonScope().AsWeakBinding();
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
            Container.Get<ILoggerService>("filelogger").Error(e.Exception);
            Container.Get<IWindowManager>().ShowMessageBox(e.Exception.Message, "Oops!");
        }
    }
}

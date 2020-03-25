namespace SimpleDICOMToolkit.IoCModules
{
    using StyletIoC;
    using ViewModels;

    internal class ViewModelModule : StyletIoCModule
    {
        protected override void Load()
        {
            Bind<IViewModelFactory>().ToAbstractFactory();
            Bind<WorklistViewModel>().ToSelf().InSingletonScope();
            Bind<QueryRetrieveViewModel>().ToSelf().InSingletonScope();
            Bind<CStoreViewModel>().ToSelf().InSingletonScope();
            Bind<CStoreSCPViewModel>().ToSelf().InSingletonScope();
            Bind<PrintViewModel>().ToSelf().InSingletonScope();
            Bind<PrintSCPViewModel>().ToSelf().InSingletonScope();
        }
    }
}

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
            Bind<WorklistSCPViewModel>().ToSelf().InSingletonScope();
            Bind<QueryRetrieveViewModel>().ToSelf().InSingletonScope();
            Bind<StoreViewModel>().ToSelf().InSingletonScope();
            Bind<StoreSCPViewModel>().ToSelf().InSingletonScope();
            Bind<PrintViewModel>().ToSelf().InSingletonScope();
            Bind<PrintSCPViewModel>().ToSelf().InSingletonScope();
        }
    }
}

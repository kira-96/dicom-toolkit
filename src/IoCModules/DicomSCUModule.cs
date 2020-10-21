namespace SimpleDICOMToolkit.IoCModules
{
    using StyletIoC;
    using Client;

    internal class DicomSCUModule : StyletIoCModule
    {
        protected override void Load()
        {
            Bind<IVerifySCU>().To<VerifySCU>().InSingletonScope().AsWeakBinding();
            Bind<IStoreSCU>().To<StoreSCU>().InSingletonScope().AsWeakBinding();
            Bind<IPrintSCU>().To<PrintSCU>().InSingletonScope().AsWeakBinding();
            Bind<IWorklistSCU>().To<WorklistSCU>().InSingletonScope().AsWeakBinding();
            Bind<IQueryRetrieveSCU>().To<QueryRetrieveSCU>().InSingletonScope().AsWeakBinding();
        }
    }
}

namespace Config
{
    using Stylet;
    using StyletIoC;
    using System.Threading;
    using ViewModels;

    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private const string MUTEX_NAME = "%CONFIG%{7AF8D164-4C73-4261-A862-188AD7ED0764}";

        private static Mutex mutex;

        protected override void OnStart()
        {
            mutex = new Mutex(true, MUTEX_NAME, out bool createNew);
            if (!createNew)
            {
                System.Environment.Exit(0);
            }

            base.OnStart();

            mutex.ReleaseMutex();
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<ShellViewModel>().ToSelf().InSingletonScope();
        }
    }
}

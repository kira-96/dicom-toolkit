namespace SimpleDICOMToolkit.ViewModels
{
    using Stylet;
    using System;
    using System.Threading.Tasks;
    using MQTT;
    using Services;

    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public static string WindowName = "Simple DICOM Toolkit";

        private readonly IConfigurationService configurationService;
        private readonly ISimpleMqttService mqttService;

        public ShellViewModel(
            IConfigurationService configurationService,
            DcmItemsViewModel dcmItemsViewModel,
            WorklistViewModel worklistViewModel,
            WorklistSCPViewModel worklistSCPViewModel,
            QueryRetrieveViewModel queryRetrieveViewModel,
            CStoreViewModel cstoreViewModel,
            CStoreSCPViewModel cstoreSCPViewModel,
            PrintViewModel printViewModel,
            PrintSCPViewModel printSCPViewModel,
            ISimpleMqttService mqttService)
        {
            DisplayName = WindowName;
            this.configurationService = configurationService;
            this.mqttService = mqttService;

            Items.Add(dcmItemsViewModel);
            Items.Add(worklistViewModel);
            Items.Add(worklistSCPViewModel);
            Items.Add(queryRetrieveViewModel);
            Items.Add(cstoreViewModel);
            Items.Add(cstoreSCPViewModel);
            Items.Add(printViewModel);
            Items.Add(printSCPViewModel);
        }

        protected override async void OnViewLoaded()
        {
            base.OnViewLoaded();

            var appConfiguration = configurationService.GetConfiguration<AppConfiguration>();

            await mqttService.StartAsync(appConfiguration.ListenPort);

            ActiveItem = Items.Count > 0 ? Items[0] : null;

            await HandleCommandLineArgs(Environment.GetCommandLineArgs());
        }

        private async Task HandleCommandLineArgs(string[] args)
        {
            if (args.Length == 2)
            {
                if (System.IO.File.Exists(args[1]))
                {
                    await (Items[0] as DcmItemsViewModel).OpenDcmFile(args[1]);
                }
            }
        }
    }
}

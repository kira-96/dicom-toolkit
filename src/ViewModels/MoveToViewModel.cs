using Stylet;
using StyletIoC;
using System;
using SimpleDICOMToolkit.Client;
using SimpleDICOMToolkit.Logging;
using SimpleDICOMToolkit.Services;

namespace SimpleDICOMToolkit.ViewModels
{
    public class MoveToViewModel : Screen
    {
        private readonly ILoggerService logger;
        private readonly II18nService i18NService;
        private readonly INotificationService notificationService;
        private readonly IVerifySCU verifySCU;

        private string serverIP = "0.0.0.0";
        private string serverPort = "104";
        private string serverAET = "STORE-SCP";

        public string ServerIP
        {
            get => serverIP;
            set
            {
                if (SetAndNotify(ref serverIP, value))
                {
                    NotifyOfPropertyChange(() => CanVerify);
                    NotifyOfPropertyChange(() => CanOnOk);
                }
            }
        }

        public string ServerPort
        {
            get => serverPort;
            set
            {
                if (SetAndNotify(ref serverPort, value))
                {
                    NotifyOfPropertyChange(() => CanVerify);
                    NotifyOfPropertyChange(() => CanOnOk);
                }
            }
        }

        public string ServerAET
        {
            get => serverAET;
            set
            {
                if (SetAndNotify(ref serverAET, value))
                {
                    NotifyOfPropertyChange(() => CanVerify);
                    NotifyOfPropertyChange(() => CanOnOk);
                }
            }
        }

        private bool isBusy = false;

        public bool IsBusy
        {
            get => isBusy;
            private set
            {
                if (SetAndNotify(ref isBusy, value))
                {
                    NotifyOfPropertyChange(() => CanVerify);
                    NotifyOfPropertyChange(() => CanOnOk);
                }
            }
        }

        public MoveToViewModel(
            [Inject("filelogger")] ILoggerService logger,
            II18nService i18NService,
            INotificationService notificationService,
            IVerifySCU verifySCU,
            IModelValidator<MoveToViewModel> validator) : base(validator)
        {
            DisplayName = "Move To";
            this.logger = logger;
            this.i18NService = i18NService;
            this.notificationService = notificationService;
            this.verifySCU = verifySCU;
        }

        public bool CanVerify =>
            !string.IsNullOrEmpty(ServerIP) &&
            !string.IsNullOrEmpty(ServerPort) &&
            !string.IsNullOrEmpty(ServerAET) &&
            !IsBusy;

        public async void Verify()
        {
            int port = ParseServerPort();
            if (port == 0)
                return;

            IsBusy = true;

            bool result = await verifySCU.VerifyAsync(ServerIP, port, serverAET, "Verify");

            IsBusy = false;

            if (result)
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("VerifySuccess"), new TimeSpan(0, 0, 3), Controls.ToastType.Info);
            }
            else
            {
                await notificationService.ShowToastAsync(i18NService.GetXmlStringByKey("VerifyFailed"), new TimeSpan(0, 0, 3), Controls.ToastType.Error);
            }
        }

        public bool CanOnOk =>
            !string.IsNullOrEmpty(ServerIP) &&
            !string.IsNullOrEmpty(ServerPort) &&
            !string.IsNullOrEmpty(ServerAET) &&
            !IsBusy;

        public void OnOk()
        {
            RequestClose(true);
        }

        public int ParseServerPort()
        {
            if (!int.TryParse(serverPort, out int port))
            {
                logger.Warn("非法的端口号：{0}，端口号必须为数字。", serverPort);
                ServerPort = "";
            }

            return port;
        }
    }
}

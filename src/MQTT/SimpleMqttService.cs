namespace SimpleDICOMToolkit.MQTT
{
    using StyletIoC;
    using MQTTnet;
    using MQTTnet.Protocol;
    using MQTTnet.Server;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Logging;

    public class SimpleMqttService : ISimpleMqttService
    {
        public int Port { get; private set; }
        private readonly ILoggerService logger;

        private readonly IMqttServer server;
        private readonly Dictionary<string, string> users;

        public SimpleMqttService([Inject(Key = "filelogger")] ILoggerService loggerService)
        {
            logger = loggerService;
            users = new Dictionary<string, string>()
            {
                { "AD*米妮*斯托蕾塔", "^P@$$W0&D$" }
            };

            server = new MqttFactory().CreateMqttServer();
            server.UseApplicationMessageReceivedHandler(e =>
            {
                string message =
                     "\r\n### RECEIVED APPLICATION MESSAGE ###\r\n" +
                    $"+ Topic = {e.ApplicationMessage.Topic}\r\n" +
                    $"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}\r\n" +
                    $"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}\r\n" +
                    $"+ Retain = {e.ApplicationMessage.Retain}\r\n";

                logger.Info(message);
            }).UseClientConnectedHandler(e => { logger.Debug($"[{e.ClientId}] Connected."); })
            .UseClientDisconnectedHandler(e => { logger.Debug($"[{e.ClientId}] Disconnected."); });
        }

        public async Task<bool> StartAsync(int port)
        {
            if (server.IsStarted)
            {
                return true;
            }

            Port = port;
            IMqttServerOptions options = new MqttServerOptionsBuilder()
                .WithApplicationMessageInterceptor(cv => { cv.AcceptPublish = true; })
                .WithConnectionValidator(cv => 
                {
                    if (cv.ClientId.Length < 10)
                    {
                        cv.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                        return;
                    }

                    if (!users.ContainsKey(cv.Username) ||
                        users[cv.Username] != cv.Password)
                    {
                        cv.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                        return;
                    }

                    cv.ReasonCode = MqttConnectReasonCode.Success;
                })
                .WithEncryptionSslProtocol(System.Security.Authentication.SslProtocols.Tls12)
                .WithDefaultEndpointPort(port)
                .WithSubscriptionInterceptor(cv => { cv.AcceptSubscription = true; })
                .Build();

            try
            {
                await server.StartAsync(options);
                logger.Debug("Mqtt server is running at: {0}", port);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task StopAsync()
        {
            if (server.IsStarted)
            {
                await server.StopAsync();
            }
        }

        public async void Dispose()
        {
            await StopAsync();
        }
    }
}

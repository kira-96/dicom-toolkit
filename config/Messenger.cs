namespace Config
{
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using NLog;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Messenger
    {
        private static Messenger _instance;
        private static readonly object _locker = new object();

        public static Messenger Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new Messenger();
                        }
                    }
                }

                return _instance;
            }
        }

        public int ServerPort { get; set; } = 9629;

        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private readonly Logger logger = LogManager.GetLogger("ConfigLogger");

        private IMqttClient client;

        public IMqttClient Client => client ?? (client = new MqttFactory().CreateMqttClient());

        public async ValueTask PublishAsync(string topic, string payload, CancellationToken token)
        {
            if (!Client.IsConnected)
            {
                if (!await TryConnectAsync())
                {
                    logger.Error("Publish message failed.");
                    return;
                }
            }

            await Client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build(), token);
        }

        private async ValueTask<bool> TryConnectAsync()
        {
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithCleanSession()
                .WithClientId("设置客户端 ID: 002")
                .WithCredentials("AD*米妮*斯托蕾塔", "^P@$$W0&D$")
                .WithTcpServer("localhost", ServerPort)
                .Build();

            Client.UseConnectedHandler(e => { logger.Debug("Successfully connected to: {0}", e.AuthenticateResult.ResultCode); })
                .UseDisconnectedHandler(async e =>
                {
                    logger.Warn("### DISCONNECTED FROM SERVER ###");
                    await Task.Delay(2000);

                    try
                    {
                        await Client.ConnectAsync(options, CancellationTokenSource.Token);
                    }
                    catch
                    {
                        logger.Error("### RECONNECTING FAILED ###");
                    }
                })
                .UseApplicationMessageReceivedHandler(e => 
                {
                    string message =
                     "\r\n### RECEIVED APPLICATION MESSAGE ###\r\n" +
                    $"+ Topic = {e.ApplicationMessage.Topic}\r\n" +
                    $"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}\r\n" +
                    $"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}\r\n" +
                    $"+ Retain = {e.ApplicationMessage.Retain}\r\n";

                    logger.Info(message);
                });

            try
            {
                await Client.ConnectAsync(options, CancellationTokenSource.Token);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Could not connect to server: {0}", ex.Message);
                return false;
            }
        }
    }
}

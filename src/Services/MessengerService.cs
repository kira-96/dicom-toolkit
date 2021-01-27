namespace SimpleDICOMToolkit.Services
{
    using GalaSoft.MvvmLight.Helpers;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using StyletIoC;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Logging;

    public class MessengerService : IMessengerService
    {
        private readonly Dictionary<string, List<WeakActionAndToken>> recipientsStrictAction = new Dictionary<string, List<WeakActionAndToken>>();
        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private readonly ILoggerService logger;
        private readonly IConfigurationService configurationService;
        private readonly IMqttClient client;

        public MessengerService([Inject(Key = "filelogger")] ILoggerService loggerService,
            IConfigurationService configurationService)
        {
            logger = loggerService;
            this.configurationService = configurationService;
            client = new MqttFactory().CreateMqttClient();
        }

        public async ValueTask PublishAsync(string topic, string payload, CancellationToken token)
        {
            if (!client.IsConnected)
            {
                if (!await TryConnectAsync())
                {
                    logger.Error("Publish message failed.");
                    return;
                }
            }

            await client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build(), token);
        }

        public async ValueTask SubscribeAsync(object recipient, string topic, Action<string> action)
        {
            if (recipient == null ||
                topic == null ||
                action == null)
            {
                return;
            }

            if (!client.IsConnected)
            {
                if (!await TryConnectAsync())
                {
                    logger.Warn("Subscribe [{0}] failed, because client not connected to a server.", topic);
                    return;
                }
            }

            await client.SubscribeAsync(topic);

            if (!recipientsStrictAction.ContainsKey(topic))
            {
                List<WeakActionAndToken> actions = new List<WeakActionAndToken>();
                WeakActionAndToken item = new WeakActionAndToken()
                {
                    Action = new WeakAction<string>(recipient, action),
                    Token = recipient.GetHashCode(),
                };
                actions.Add(item);
                recipientsStrictAction.Add(topic, actions);
            }
            else
            {
                recipientsStrictAction[topic].Add(
                    new WeakActionAndToken()
                    {
                        Action = new WeakAction<string>(recipient, action),
                        Token = recipient.GetHashCode()
                    });
            }
        }

        public async ValueTask UnsubscribeAsync(object recipient, string topic)
        {
            if (recipient == null ||
                !recipientsStrictAction.ContainsKey(topic))
            {
                return;
            }

            if (!client.IsConnected)
            {
                logger.Warn("Unsubscribe [{0}] failed, because client not connected to a server.", topic);
                return;
            }

            await client.UnsubscribeAsync(topic);

            lock (recipient)
            {
                foreach (var item in recipientsStrictAction[topic])
                {
                    if (item.Action is WeakAction<string> weakAction && 
                        recipient == weakAction.Target && 
                        recipient.GetHashCode() == item.Token)
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }

        private async ValueTask<bool> TryConnectAsync()
        {
            MiscConfiguration misc = configurationService.GetConfiguration<MiscConfiguration>();

            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithCleanSession()
                .WithClientId("主客户端 ID: 001")
                .WithCredentials("AD*米妮*斯托蕾塔", "^P@$$W0&D$")
                .WithTcpServer("localhost", misc.ListenPort)
                .Build();

            client.UseConnectedHandler(e => { logger.Debug("Successfully connected to: {0}", e.AuthenticateResult.ResultCode); })
                .UseDisconnectedHandler(async e =>
                {
                    logger.Warn("### DISCONNECTED FROM SERVER ###");
                    await Task.Delay(2000);

                    try
                    {
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            logger.Debug("### CancellationRequested ###");
                            return;
                        }
                        await client.ConnectAsync(options, CancellationTokenSource.Token);
                    }
                    catch
                    {
                        logger.Error("### RECONNECTING FAILED ###");
                    }
                })
                .UseApplicationMessageReceivedHandler(e =>
                {
#if DEBUG
                    string message =
                     "\r\n### RECEIVED APPLICATION MESSAGE ###\r\n" +
                     $"+ Topic = {e.ApplicationMessage.Topic}\r\n" +
                     $"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}\r\n" +
                     $"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}\r\n" +
                     $"+ Retain = {e.ApplicationMessage.Retain}\r\n";

                    System.Diagnostics.Debug.WriteLine(message);
#endif
                    ExecuteMessage(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                });

            try
            {
                await client.ConnectAsync(options, CancellationTokenSource.Token);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Could not connect to server: {0}", ex.Message);
                return false;
            }
        }

        private void ExecuteMessage(string topic, string payload)
        {
            if (topic == null) return;

            if (!recipientsStrictAction.ContainsKey(topic))
                return;

            List<WeakActionAndToken> weakActionAndTokens = recipientsStrictAction[topic];

            foreach (var item in weakActionAndTokens)
            {
                if (item.Action is IExecuteWithObject executeAction)
                {
                    executeAction.ExecuteWithObject(payload);
                }
            }
        }

        public void Dispose()
        {
            CancellationTokenSource.Cancel();
            if (client.IsConnected)
            {
                client.DisconnectAsync();
                client.Dispose();
            }
        }
    }
}

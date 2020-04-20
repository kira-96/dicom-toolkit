using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.MQTT
{
    public interface IMessenger
    {
        Task PublishAsync(string topic, string payload, CancellationToken token);

        Task SubscribeAsync(object recipient, string topic, Action<string> action);

        Task UnsubscribeAsync(object recipient, string topic);
    }
}

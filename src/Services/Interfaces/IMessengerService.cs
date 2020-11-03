using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface IMessengerService : IDisposable
    {
        ValueTask PublishAsync(string topic, string payload, CancellationToken token);

        ValueTask SubscribeAsync(object recipient, string topic, Action<string> action);

        ValueTask UnsubscribeAsync(object recipient, string topic);
    }
}

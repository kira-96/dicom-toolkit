using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface IMessengerService : IDisposable
    {
        Task PublishAsync(string topic, string payload, CancellationToken token);

        Task SubscribeAsync(object recipient, string topic, Action<string> action);

        Task UnsubscribeAsync(object recipient, string topic);
    }
}

using System;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface ISimpleMqttService : IDisposable
    {
        int Port { get; }

        ValueTask<bool> StartAsync(int port);

        ValueTask StopAsync();
    }
}

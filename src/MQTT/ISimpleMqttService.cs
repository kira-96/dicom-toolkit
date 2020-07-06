using System;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.MQTT
{
    public interface ISimpleMqttService : IDisposable
    {
        int Port { get; }

        Task<bool> StartAsync(int port);

        Task StopAsync();
    }
}

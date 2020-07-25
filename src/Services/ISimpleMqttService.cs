using System;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface ISimpleMqttService : IDisposable
    {
        int Port { get; }

        Task<bool> StartAsync(int port);

        Task StopAsync();
    }
}

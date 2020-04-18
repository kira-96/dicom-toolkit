using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public interface ISimpleMqttService
    {
        int Port { get; }

        Task StartAsync(int port);

        Task StopAsync();
    }
}

using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Services
{
    public class SimpleMqttService : ISimpleMqttService
    {
        // private readonly IMqttServer _mqttServer;

        public int Port { get; private set; }

        public SimpleMqttService()
        {
            //
        }

        public Task StartAsync(int port)
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}

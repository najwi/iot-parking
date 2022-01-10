using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;

namespace iot_parking.Services
{
    public interface IMqttClientService : IHostedService,
                                          IMqttClientConnectedHandler,
                                          IMqttClientDisconnectedHandler,
                                          IMqttApplicationMessageReceivedHandler
    {

    }
}

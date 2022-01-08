using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace iot_parking.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient _mqttClient;
        private IMqttClientOptions _options;

        private const string EntryGatesTopic = "gate/e/";
        private const string LeaveGatesTopic = "gate/l/";
        private const string CardReaderTopic = "reader/";

        public MqttClientService(IMqttClientOptions options)
        {
            this._options = options;
            _mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            _mqttClient.ConnectedHandler = this;
            _mqttClient.DisconnectedHandler = this;
            _mqttClient.ApplicationMessageReceivedHandler = this;

            //mqttClient.UseApplicationMessageReceivedHandler(async e =>
            //{
            //    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            //    Console.WriteLine($"+ ClientId = {e.ClientId}");
            //    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            //    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //    Console.WriteLine($"+ SubscriptionIdentifiers = {e.ApplicationMessage.SubscriptionIdentifiers}");
            //    Console.WriteLine($"+ MessageExpiryInterval = {e.ApplicationMessage.MessageExpiryInterval}");
            //    Console.WriteLine($"+ ContentType = {e.ApplicationMessage.ContentType}");
            //    Console.WriteLine($"+ AutoAcknowledge = {e.AutoAcknowledge}");
            //    Console.WriteLine($"+ IsHandled = {e.IsHandled}");
            //    Console.WriteLine($"+ ReasonCode = {e.ReasonCode}");
            //    Console.WriteLine();

            //    await Task.Run(() => mqttClient.PublishAsync("hello/world"));
            //});
            //mqttClient.UseApplicationMessageReceivedHandler(HandleApplicationMessageReceivedAsync);
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            Console.WriteLine("### CONNECTED WITH SERVER ###");
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(EntryGatesTopic + '+').Build());
            Console.WriteLine($"### SUBSCRIBED TO {EntryGatesTopic}+ ###");
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(LeaveGatesTopic + '+').Build());
            Console.WriteLine($"### SUBSCRIBED TO {LeaveGatesTopic}+ ###");
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(CardReaderTopic + '+').Build());
            Console.WriteLine($"### SUBSCRIBED TO {CardReaderTopic}+ ###");
        }

        public Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            return Task.Delay(1);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("### CONNECTING TO SERVER ###");
            await _mqttClient.ConnectAsync(_options);
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ReconnectAsync();
            }
        }

        private Task HandleEntryGateMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {

        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string messageTopic = eventArgs.ApplicationMessage.Topic;
            Console.WriteLine($"Received message topic: {messageTopic}");
            string messageType = messageTopic.Substring(0, messageTopic.LastIndexOf('/') + 1);
            Console.WriteLine(messageType);
            switch (messageType)
            {
                case EntryGatesTopic:
                    break;
                case LeaveGatesTopic:
                    break;
                case CardReaderTopic:
                    break;
                default:
                    Console.WriteLine("Unhandled message topic");
                    break;
            }
            return Task.Delay(1);
        }

        public async Task SendMessage(string message) 
        {
            await _mqttClient.PublishAsync(message);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                var disconnectOption = new MqttClientDisconnectOptions
                {
                    ReasonCode = MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "NormalDiconnection"
                };
                await _mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
            }
            await _mqttClient.DisconnectAsync();
        }
    }
}

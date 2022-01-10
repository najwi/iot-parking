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

using iot_parking.Database;
using Microsoft.Extensions.DependencyInjection;

namespace iot_parking.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;

        private const string EntryGatesTopic = "gate/e/";
        private const string LeaveGatesTopic = "gate/l/";
        private const string CardReaderTopic = "reader/";

        private const string OpenGateSuccessMessage = "success";
        private const string OpenGateFailureMessage = "failure";

        public MqttClientService(IMqttClientOptions options, IServiceScopeFactory scopeFactory)
        {
            this._options = options;
            _scopeFactory = scopeFactory;
            _mqttClient = new MqttFactory().CreateMqttClient();
            ConfigureMqttClient();
        }

        private void ConfigureMqttClient()
        {
            _mqttClient.ConnectedHandler = this;
            _mqttClient.DisconnectedHandler = this;
            _mqttClient.ApplicationMessageReceivedHandler = this;
        }

        private async Task SendGateResponse(string messageTopic, DbResponse gateMessage)
        {
            byte[] bytes;
            bytes = Encoding.UTF8.GetBytes($"status:{gateMessage};");
            string ResponseTopic = $"{messageTopic}/r";

            MqttApplicationMessageBuilder message = new MqttApplicationMessageBuilder()
                    .WithTopic(messageTopic + "/r")
                    .WithRetainFlag(false)
                    .WithExactlyOnceQoS()
                    .WithPayload(bytes);
            await _mqttClient.PublishAsync(message.Build());
        }

        private string GetCardId(string payload, string parameter = "card")
        {
            string[] cardParameter = payload.Split(':', ';');

            return cardParameter[Array.IndexOf(cardParameter, parameter) + 1];
        }

        private async Task HandleEntryGateMessageReceivedAsync(string messageTopic, string messagePayload)
        {
            string clientId = messageTopic.Substring(messageTopic.LastIndexOf('/') + 1);
            string cardNumber = GetCardId(messagePayload);
            Console.WriteLine($"Received card RFID: {cardNumber}");
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                DbResponse message = await db.CheckEntry(clientId, cardNumber);
                Console.WriteLine($"Open gate: {message}");
                await SendGateResponse(messageTopic, message);
            }
        }

        private async Task HandleLeaveGateMessageReceivedAsync(string messageTopic, string messagePayload)
        {
            string clientId = messageTopic.Substring(messageTopic.LastIndexOf('/') + 1);
            string cardNumber = GetCardId(messagePayload);
            Console.WriteLine($"Received card RFID: {cardNumber}");
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                DbResponse message = await db.CheckLeave(clientId, cardNumber);
                Console.WriteLine($"Open gate: {message}");
                await SendGateResponse(messageTopic, message);
            }
        }

        private async Task HandleCardReaderMessageReceivedAsync(string messageTopic, string messagePayload)
        {
            string clientId = messageTopic.Substring(messageTopic.LastIndexOf('/') + 1);
            string cardNumber = GetCardId(messagePayload);
            Console.WriteLine($"Received card RFID: {cardNumber}");
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                // Todo
                // Save new card for later registration
            }
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string messageTopic = eventArgs.ApplicationMessage.Topic;
            Console.WriteLine($"Received message topic: {messageTopic}");
            string messageType = messageTopic.Substring(0, messageTopic.LastIndexOf('/') + 1);
            Console.WriteLine(messageType);
            string messagePayload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            switch (messageType)
            {
                case EntryGatesTopic:
                    await HandleEntryGateMessageReceivedAsync(messageTopic, messagePayload);
                    break;
                case LeaveGatesTopic:
                    await HandleLeaveGateMessageReceivedAsync(messageTopic, messagePayload);
                    break;
                case CardReaderTopic:
                    await HandleCardReaderMessageReceivedAsync(messageTopic, messagePayload);
                    break;
                default:
                    Console.WriteLine($"Unhandled message topic: {messageTopic}");
                    break;
            }
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

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await _mqttClient.ConnectAsync(_options, CancellationToken.None); 
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ReconnectAsync(CancellationToken.None);
                }
            }
            catch
            {
                Console.WriteLine("### RECONNECTING TO SERVER FAILED ###");
            }

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("### CONNECTING TO SERVER ###");
            try
            {
                await _mqttClient.ConnectAsync(_options, CancellationToken.None);
                if (!_mqttClient.IsConnected)
                {
                    await _mqttClient.ReconnectAsync(CancellationToken.None);
                }
            }
            catch
            {
                Console.WriteLine("### CONNECTING TO SERVER FAILED ###");
            }
        }

        // Utility test function
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

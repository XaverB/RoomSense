using MQTTnet.Client;
using MQTTnet;
using RoomSense_Backend.Message;

namespace RoomSense_Backend.Service
{
    public class MqttConnectionService : IMqttService
    {
        private readonly ILogger<IMqttService> _logger;
        private IMqttClient? _mqttClient;
        private readonly MqttConnectionOptions _options;
        private readonly IMessageProcessor _messageProcessor;

        public MqttConnectionService(ILogger<IMqttService> logger, MqttConnectionOptions options, IMessageProcessor messageProcessor)
        {
            _logger = logger;
            _options = options;
            _messageProcessor = messageProcessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += _messageProcessor.HandleMessageReceivedAsync;

            await _mqttClient.ConnectAsync(_options.BuildOptions(), cancellationToken);

            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("room/+/humidity")
                .WithTopic("room/+/temperature")
                .WithTopic("room/+/co2")
                .Build();

            await _mqttClient.SubscribeAsync(topicFilter, cancellationToken);

            _logger.LogInformation("MQTT client connected and subscribed to topics.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient?.IsConnected == true)
            {
                await _mqttClient.DisconnectAsync();
                _logger.LogInformation("MQTT client disconnected.");
            }
        }
    }
}

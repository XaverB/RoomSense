using MQTTnet.Client;
using System.Text.Json;
using System.Text;

namespace RoomSense_Backend.Message
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;

        public MessageProcessor(ILogger<MessageProcessor> logger)
        {
            _logger = logger;
        }

        public async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
            var topic = eventArgs.ApplicationMessage.Topic;

            _logger.LogInformation("Received MQTT message on topic: {Topic}", topic);

            var segments = topic.Split('/');
            if (segments.Length == 3)
            {
                var roomName = segments[1];
                var sensorType = segments[2];

                switch (sensorType)
                {
                    case "humidity":
                        await ProcessHumidityMessage(roomName, message);
                        break;
                    case "temperature":
                        await ProcessTemperatureMessage(roomName, message);
                        break;
                    case "co2":
                        await ProcessCo2Message(roomName, message);
                        break;
                }
            }
        }

        private Task ProcessHumidityMessage(string roomName, string message)
        {
            var payload = JsonSerializer.Deserialize<Payload>(message);
            _logger.LogInformation("Humidity in room {RoomName}: {Value} (Device: {Device}, Timestamp: {Timestamp})",
                roomName, payload.Value, payload.Device, payload.Timestamp);
            // Process humidity data for the specific room
            return Task.CompletedTask;
        }

        private Task ProcessTemperatureMessage(string roomName, string message)
        {
            var payload = JsonSerializer.Deserialize<Payload>(message);
            _logger.LogInformation("Temperature in room {RoomName}: {Value} (Device: {Device}, Timestamp: {Timestamp})",
                roomName, payload.Value, payload.Device, payload.Timestamp);
            // Process temperature data for the specific room
            return Task.CompletedTask;
        }

        private Task ProcessCo2Message(string roomName, string message)
        {
            var payload = JsonSerializer.Deserialize<Payload>(message);
            _logger.LogInformation("CO2 in room {RoomName}: {Value} (Device: {Device}, Timestamp: {Timestamp})",
                roomName, payload.Value, payload.Device, payload.Timestamp);
            // Process CO2 data for the specific room
            return Task.CompletedTask;
        }
    }
}

// Ignore Spelling: Mqtt

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using RoomSense_Backend.Message;

namespace RoomSense_Backend.Service
{

    public class MqttService : IHostedService
    {
        private readonly ILogger<MqttService> _logger;
        private IMqttClient _mqttClient;

        private readonly string MQTT_BROKER_URL;
        private readonly int MQTT_BROKER_PORT = 1883;
        private readonly string MQTT_BROKER_USERNAME;
        private readonly string MQTT_BROKER_PASSWORD;

        public MqttService(ILogger<MqttService> logger)
        {
            _logger = logger;

            MQTT_BROKER_URL = Environment.GetEnvironmentVariable("MQTT_BROKER_URL") ?? throw new ArgumentNullException("MQTT_BROKER_URL not defined as environment variable");
            MQTT_BROKER_USERNAME = Environment.GetEnvironmentVariable("MQTT_BROKER_USERNAME") ?? throw new ArgumentNullException("MQTT_BROKER_USERNAME not defined as environment variable");
            MQTT_BROKER_PASSWORD = Environment.GetEnvironmentVariable("MQTT_BROKER_PASSWORD") ?? throw new ArgumentNullException("MQTT_BROKER_PASSWORD not defined as environment variable");
            string mqttBrokerPort = Environment.GetEnvironmentVariable("MQTT_BROKER_PORT") ?? throw new ArgumentNullException("MQTT_BROKER_PORT not defined as environment variable");
            MQTT_BROKER_PORT = int.Parse(mqttBrokerPort);

            _logger.LogInformation($"{nameof(MqttService)} constructed with MQTT_BROKER_URL: {MQTT_BROKER_URL}:{MQTT_BROKER_URL}@{MQTT_BROKER_USERNAME}");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(MQTT_BROKER_URL, MQTT_BROKER_PORT)
                .WithCredentials(MQTT_BROKER_USERNAME, MQTT_BROKER_PASSWORD)
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;

            await _mqttClient.ConnectAsync(options, cancellationToken);

            var topicFilter = new MqttTopicFilterBuilder()
                       .WithTopic("room/+/humidity")
                       .WithTopic("room/+/temperature")
                       .WithTopic("room/+/co2")
                       .Build();

            await _mqttClient.SubscribeAsync(topicFilter, cancellationToken);

            _logger.LogInformation("MQTT client connected and subscribed to topic.");
        }

        private async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
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

using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorSimulator
{
    public class SensorSimulator
    {
        private readonly IMqttClient _mqttClient;
        private readonly string mqttServerAddress;
        private readonly int port;
        private readonly string _mqttTopic;
        private readonly string username;
        private readonly string password;
        private readonly Random _random;

        public SensorSimulator(string mqttServerAddress, int port, string mqttTopic, string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
            this.mqttServerAddress = mqttServerAddress ?? throw new ArgumentNullException(nameof(mqttServerAddress));
            this.port = port;
            _mqttTopic = mqttTopic ?? throw new ArgumentNullException(nameof(mqttTopic));
            this.username = username;
            this.password = password;
            _random = new Random();
        }

        public async Task ConnectAsync()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttServerAddress, port)
                .WithCredentials(username, password)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = true, // Set this to false in production and use a trusted certificate
                    IgnoreCertificateChainErrors = true, // Set this to false in production
                    IgnoreCertificateRevocationErrors = true // Set this to false in production
                })
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            await _mqttClient.ConnectAsync(mqttClientOptions);
        }

        public async Task SimulateAndPublishAsync(int interval)
        {
            double temperature = 20.0; // Starting temperature value
            double humidity = 50.0; // Starting humidity value
            double co2 = 400.0; // Starting CO2 value

            double temperatureTrend = 0.1; // Temperature trend factor
            double humidityTrend = 0.2; // Humidity trend factor
            double co2Trend = 1.0; // CO2 trend factor

            while (true)
            {
                try
                {
                    if (!_mqttClient.IsConnected)
                    {
                        await ConnectAsync();
                    }

                    // Generate temperature data with trend
                    temperature += _random.NextDouble() * 2 * temperatureTrend - temperatureTrend;
                    temperature = Math.Max(0, Math.Min(100, temperature)); // Ensure temperature is within 0-100 range

                    var temperatureData = new SensorData
                    {
                        device = username,
                        value = Math.Round(temperature, 2).ToString(),
                        timestamp = DateTime.UtcNow
                    };

                    var temperatureMessage = new MqttApplicationMessageBuilder()
                        .WithTopic($"{_mqttTopic}/temperature")
                        .WithPayload(JsonConvert.SerializeObject(temperatureData))
                        .Build();

                    await _mqttClient.PublishAsync(temperatureMessage);
                    Console.WriteLine($"Published temperature data: {temperatureData.value}");

                    // Generate humidity data with trend
                    humidity += _random.NextDouble() * 2 * humidityTrend - humidityTrend;
                    humidity = Math.Max(0, Math.Min(100, humidity)); // Ensure humidity is within 0-100 range

                    var humidityData = new SensorData
                    {
                        device = username,
                        value = Math.Round(humidity, 2).ToString(),
                        timestamp = DateTime.UtcNow
                    };

                    var humidityMessage = new MqttApplicationMessageBuilder()
                        .WithTopic($"{_mqttTopic}/humidity")
                        .WithPayload(JsonConvert.SerializeObject(humidityData))
                        .Build();

                    await _mqttClient.PublishAsync(humidityMessage);
                    Console.WriteLine($"Published humidity data: {humidityData.value}");

                    // Generate CO2 data with trend
                    co2 += _random.NextDouble() * 2 * co2Trend - co2Trend;
                    co2 = Math.Max(300, Math.Min(2000, co2)); // Ensure CO2 is within 300-2000 range

                    var co2Data = new SensorData
                    {
                        device = username,
                        value = Math.Round(co2, 2).ToString(),
                        timestamp = DateTime.UtcNow
                    };

                    var co2Message = new MqttApplicationMessageBuilder()
                        .WithTopic($"{_mqttTopic}/co2")
                        .WithPayload(JsonConvert.SerializeObject(co2Data))
                        .Build();

                    await _mqttClient.PublishAsync(co2Message);
                    Console.WriteLine($"Published CO2 data: {co2Data.value}");

                    await Task.Delay(TimeSpan.FromSeconds(interval));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unhandled exception", ex);
                }
            }
        }
    }
}

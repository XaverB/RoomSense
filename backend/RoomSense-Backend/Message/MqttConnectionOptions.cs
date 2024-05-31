using MQTTnet.Client;

namespace RoomSense_Backend.Message
{
    public class MqttConnectionOptions
    {
        private readonly string _brokerUrl;
        private readonly int _brokerPort;
        private readonly string _brokerUsername;
        private readonly string _brokerPassword;

        public MqttConnectionOptions()
        {
            _brokerUrl = Environment.GetEnvironmentVariable("MQTT_BROKER_URL") ?? throw new ArgumentNullException("MQTT_BROKER_URL not defined as environment variable");
            _brokerUsername = Environment.GetEnvironmentVariable("MQTT_BROKER_USERNAME") ?? throw new ArgumentNullException("MQTT_BROKER_USERNAME not defined as environment variable");
            _brokerPassword = Environment.GetEnvironmentVariable("MQTT_BROKER_PASSWORD") ?? throw new ArgumentNullException("MQTT_BROKER_PASSWORD not defined as environment variable");
            string mqttBrokerPort = Environment.GetEnvironmentVariable("MQTT_BROKER_PORT") ?? throw new ArgumentNullException("MQTT_BROKER_PORT not defined as environment variable");
            _brokerPort = int.Parse(mqttBrokerPort);
        }

        public MqttClientOptions BuildOptions()
        {
            return new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerUrl, _brokerPort)
                .WithCredentials(_brokerUsername, _brokerPassword)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = true,
                    AllowUntrustedCertificates = true, // Set this to false in production and use a trusted certificate
                    IgnoreCertificateChainErrors = true, // Set this to false in production
                    IgnoreCertificateRevocationErrors = true // Set this to false in production
                })
                .Build();
        }
    }
}

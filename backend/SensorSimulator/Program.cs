using SensorSimulator;
using System.Diagnostics;

string mqttServerAddress;
string mqttServerPort;
string mqttTopic;
string username;
string password;

//if (Debugger.IsAttached)
//{
//    // Hardcoded values for debugging
//    mqttServerAddress = "c93e99eaf93b4f81aa4db3ba6fee2780.s1.eu.hivemq.cloud";
//    mqttServerPort = "8883";
//    mqttTopic = "room/simulator2";
//    username = "sensor-simulator-2";
//    password = "SensorSim123";
//}
//else
//{
    // Get values from environment variables
    mqttServerAddress = Environment.GetEnvironmentVariable("MQTT_BROKER_URL") ?? throw new Exception("MQTT_BROKER_URL not defined as environment variable");
    mqttServerPort = Environment.GetEnvironmentVariable("MQTT_BROKER_PORT") ?? throw new Exception("MQTT_BROKER_PORT not defined as environment variable");
    mqttTopic = Environment.GetEnvironmentVariable("MQTT_TOPIC") ?? throw new Exception("MQTT_TOPIC not defined as environment variable");
    username = Environment.GetEnvironmentVariable("MQTT_BROKER_USERNAME") ?? throw new Exception("MQTT_USERNAME not defined as environment variable");
    password = Environment.GetEnvironmentVariable("MQTT_BROKER_PASSWORD") ?? throw new Exception("MQTT_PASSWORD not defined as environment variable");
//}




var publishInterval = 30; // Publish interval in seconds

var simulator = new SensorSimulator.SensorSimulator(mqttServerAddress, int.Parse(mqttServerPort), mqttTopic, username, password);
await simulator.ConnectAsync();
await simulator.SimulateAndPublishAsync(publishInterval);
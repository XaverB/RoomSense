#include <DHT11.h>
#include "WiFiS3.h"
#include <PubSubClient.h>

DHT11 dht11(7);
int smokeA0 = A5;
int led = 12;
int threshold = 300;

const char* ssid = "LiwestD68191";
const char* password = "tq8ttt0sn1";


char mqttServerAddress[] = "test.mosquitto.org";
int mqttServerPort = 1883;

int status = WL_IDLE_STATUS;
int msgCounter = 0; 

WiFiClient client;
PubSubClient mqttClient(client);

void setup() {
  pinMode(led, OUTPUT);
  digitalWrite(led, LOW);
  Serial.begin(9600);
  setup_wifi();

  mqttClient.setServer(mqttServerAddress, mqttServerPort);
  mqttClient.setCallback(callback);
}

void setup_wifi() {
  delay(10);
  // Start the WiFi connection
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

void getTempAndHumidity() {
  int temperature = 0;
  int humidity = 0;

  int result = dht11.readTemperatureHumidity(temperature, humidity);
  if (result == 0) {
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.print(" °C\tHumidity: ");
    Serial.print(humidity);
    Serial.println(" %");

    char tempString[8];
    dtostrf(temperature, 1, 2, tempString);
    char humString[8];
    dtostrf(humidity, 1, 2, humString);
    
    mqttClient.publish("S2310454012/temperature", tempString);
    mqttClient.publish("S2310454012/humidity", humString);
    message_published_serial("temperature ", tempString);
    message_published_serial("humidity ", humString);
  } else {
    Serial.println(DHT11::getErrorString(result));
  }
}

void getCo2() {
  int analogSensor = analogRead(smokeA0);

  Serial.print("Pin A0: ");
  Serial.println(analogSensor);

  if (analogSensor > threshold) {
    digitalWrite(led, HIGH);
  } else {
    digitalWrite(led, LOW);
  }

  char co2String[8];
  dtostrf(analogSensor, 1, 2, co2String);
  mqttClient.publish("S2310454012/co2", co2String);
  message_published_serial("CO2 ", co2String);
}

void loop() {

 if (!mqttClient.connected()) {
    reconnect();
  }
  // interact with broker - ermöglicht Nebenläufigkeit
  mqttClient.loop();

  getTempAndHumidity();
  getCo2();
  delay(2000); // Delay to avoid flooding the MQTT broker
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i=0;i<length;i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
}

void message_published_serial(char* topic, char* message){
      Serial.print("message published: ");
      Serial.print(topic);
      Serial.println(message);
}

// reconnect logic for MQTT
void reconnect() {
  // Loop until we're reconnected
  while (!mqttClient.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Attempt to connect
	// this must be a unique ID for every client!!!
    if (mqttClient.connect("S2310454012-Arduino-project")) {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(mqttClient.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

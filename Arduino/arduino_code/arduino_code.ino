#include <DHT11.h>
#include "WiFiS3.h"
#include "WiFiSSLClient.h"
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include "RTC.h"

DHT11 dht11(7);
int smokeA0 = A0;
int led = 12;
int threshold = 300;

const char* ssid = "___";
const char* password = "___";

const char* mqtt_user = "device";
const char* mqtt_pw = "Device123";

char mqttServerAddress[] = "c93e99eaf93b4f81aa4db3ba6fee2780.s1.eu.hivemq.cloud";
int mqttServerPort = 8883;

const char* server_cert PROGMEM = R"EOF(
-----BEGIN CERTIFICATE-----
MIIFazCCA1OgAwIBAgIRAIIQz7DSQONZRGPgu2OCiwAwDQYJKoZIhvcNAQELBQAw
TzELMAkGA1UEBhMCVVMxKTAnBgNVBAoTIEludGVybmV0IFNlY3VyaXR5IFJlc2Vh
cmNoIEdyb3VwMRUwEwYDVQQDEwxJU1JHIFJvb3QgWDEwHhcNMTUwNjA0MTEwNDM4
WhcNMzUwNjA0MTEwNDM4WjBPMQswCQYDVQQGEwJVUzEpMCcGA1UEChMgSW50ZXJu
ZXQgU2VjdXJpdHkgUmVzZWFyY2ggR3JvdXAxFTATBgNVBAMTDElTUkcgUm9vdCBY
MTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAK3oJHP0FDfzm54rVygc
h77ct984kIxuPOZXoHj3dcKi/vVqbvYATyjb3miGbESTtrFj/RQSa78f0uoxmyF+
0TM8ukj13Xnfs7j/EvEhmkvBioZxaUpmZmyPfjxwv60pIgbz5MDmgK7iS4+3mX6U
A5/TR5d8mUgjU+g4rk8Kb4Mu0UlXjIB0ttov0DiNewNwIRt18jA8+o+u3dpjq+sW
T8KOEUt+zwvo/7V3LvSye0rgTBIlDHCNAymg4VMk7BPZ7hm/ELNKjD+Jo2FR3qyH
B5T0Y3HsLuJvW5iB4YlcNHlsdu87kGJ55tukmi8mxdAQ4Q7e2RCOFvu396j3x+UC
B5iPNgiV5+I3lg02dZ77DnKxHZu8A/lJBdiB3QW0KtZB6awBdpUKD9jf1b0SHzUv
KBds0pjBqAlkd25HN7rOrFleaJ1/ctaJxQZBKT5ZPt0m9STJEadao0xAH0ahmbWn
OlFuhjuefXKnEgV4We0+UXgVCwOPjdAvBbI+e0ocS3MFEvzG6uBQE3xDk3SzynTn
jh8BCNAw1FtxNrQHusEwMFxIt4I7mKZ9YIqioymCzLq9gwQbooMDQaHWBfEbwrbw
qHyGO0aoSCqI3Haadr8faqU9GY/rOPNk3sgrDQoo//fb4hVC1CLQJ13hef4Y53CI
rU7m2Ys6xt0nUW7/vGT1M0NPAgMBAAGjQjBAMA4GA1UdDwEB/wQEAwIBBjAPBgNV
HRMBAf8EBTADAQH/MB0GA1UdDgQWBBR5tFnme7bl5AFzgAiIyBpY9umbbjANBgkq
hkiG9w0BAQsFAAOCAgEAVR9YqbyyqFDQDLHYGmkgJykIrGF1XIpu+ILlaS/V9lZL
ubhzEFnTIZd+50xx+7LSYK05qAvqFyFWhfFQDlnrzuBZ6brJFe+GnY+EgPbk6ZGQ
3BebYhtF8GaV0nxvwuo77x/Py9auJ/GpsMiu/X1+mvoiBOv/2X/qkSsisRcOj/KK
NFtY2PwByVS5uCbMiogziUwthDyC3+6WVwW6LLv3xLfHTjuCvjHIInNzktHCgKQ5
ORAzI4JMPJ+GslWYHb4phowim57iaztXOoJwTdwJx4nLCgdNbOhdjsnvzqvHu7Ur
TkXWStAmzOVyyghqpZXjFaH3pO3JLF+l+/+sKAIuvtd7u+Nxe5AW0wdeRlN8NwdC
jNPElpzVmbUq4JUagEiuTDkHzsxHpFKVK7q4+63SM1N95R1NbdWhscdCb+ZAJzVc
oyi3B43njTOQ5yOf+1CceWxG1bQVs5ZufpsMljq4Ui0/1lvh+wjChP4kqKOJ2qxq
4RgqsahDYVvTH9w7jXbyLeiNdd8XM2w9U/t7y0Ff/9yi0GE44Za4rF2LN9d11TPA
mRGunUHBcnWEvgJBQl9nJEiU0Zsnvgc/ubhPgXRR4Xq37Z0j4r7g1SgEEzwxA57d
emyPxgcYxn/eR44/KJ4EBs+lVDR3veyJm+kXQ99b21/+jh5Xos1AnX5iItreGCc=
-----END CERTIFICATE-----

)EOF";

int status = WL_IDLE_STATUS;
int msgCounter = 0; 

WiFiSSLClient client;
PubSubClient mqttClient(client);

void setup() {
  pinMode(led, OUTPUT);
  digitalWrite(led, LOW);
  Serial.begin(9600);
  setup_wifi();
  client.setCACert(server_cert);

  RTC.begin();
  RTCTime startTime(23, Month::JUNE, 2024, 15, 9, 00, DayOfWeek::SUNDAY, SaveLight::SAVING_TIME_ACTIVE);
  RTC.setTime(startTime);

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

    pub("room/epic_mus/temperature", tempString, "temperature");
    pub("room/epic_mus/humidity", humString, "humidity");
    message_published_serial("temperature ", tempString);
    message_published_serial("humidity ", humString);
  } else {
    Serial.println(DHT11::getErrorString(result));
  }
}

void pub(char* topic, char* value, char* device){
  // Create JSON object
    StaticJsonDocument<200> doc;
    doc["device"] = device;
    doc["value"] = value;


    RTCTime currentTime;
    RTC.getTime(currentTime);
    int currentYear = currentTime.getYear();
    int currentMonth = Month2int(currentTime.getMonth());
    int currentDay = currentTime.getDayOfMonth();
    int currentHour = currentTime.getHour();
    int currentMinute = currentTime.getMinutes();
    int currentSecond = currentTime.getSeconds();

    // Format the date and time as an ISO 8601 string
    char isoDateTime[21];
    snprintf(isoDateTime, sizeof(isoDateTime), "%02d-%02d-%02dT%02d:%02d:%02dZ", currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond);
    Serial.println(isoDateTime);
    doc["timestamp"] = isoDateTime;

    // Serialize JSON to string
    char jsonBuffer[512];
    serializeJson(doc, jsonBuffer);

    mqttClient.publish(topic, jsonBuffer);

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
  pub("room/epic_mus/co2", co2String, "co2");
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
    //setup_wifi();
    // Attempt to connect
	// this must be a unique ID for every client!!!
    if (mqttClient.connect("RoomSense", mqtt_user, mqtt_pw)) {
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

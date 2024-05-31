#include <DHT11.h>

DHT11 dht11(7);
int smokeA0 = A5;
int led = 12;
int threshold = 300;
void setup() {
    pinMode(led, OUTPUT);
    digitalWrite(led, LOW);
    Serial.begin(9600);
}


void getTempAndHumidity(){
    int temperature = 0;
    int humidity = 0;

    int result = dht11.readTemperatureHumidity(temperature, humidity);
    if (result == 0) {
        Serial.print("Temperature: ");
        Serial.print(temperature);
        Serial.print(" °C\tHumidity: ");
        Serial.print(humidity);
        Serial.println(" %");
    } else {
        Serial.println(DHT11::getErrorString(result));
    }
}

void getCo2(){
  int analogSensor = analogRead(A0);

  Serial.print("Pin A0: ");
  Serial.println(analogSensor);

  if (analogSensor > threshold)
  {
    digitalWrite(led, HIGH);
  }
  else
  {
    digitalWrite(led, LOW);
  }
}

void loop() {
    getTempAndHumidity();
    getCo2();
}

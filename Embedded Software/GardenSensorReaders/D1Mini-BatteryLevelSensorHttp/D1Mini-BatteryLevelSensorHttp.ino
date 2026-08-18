/*
  Ant290
  https://github.com/ant290/Samwise
*/

#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>
#include <DHT.h>

#define sensivity (4.2 / 4095.0) //4095 //1023
#define maxVoltage 4.2

const int deviceID = 3;
const char* ssid = "NETWORK";
const char* password = "PASSWORD";

//Your Domain name with URL path or IP address with path
const char* apiAddress = "http://IP:5010/api/gardensensor";

const int sensorPowerPin = 2;

const int batteryPin = 36;

//AO pin on moisture reader
const int analogMoisturePin = 34;

//DO pin on moisture reader
const int digitalMoisturePin = 35;

//Data pin on dht22
const int dhtDataPin = 4;

//DHT type
const int dhtType = 22;

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
unsigned long lastTime = 0;
// Timer set to 15 minutes (900000)
//unsigned long timerDelay = 900000;
// Set timer to 5 seconds (5000)
unsigned long timerDelay = 300000;

// Initialise DHT sensor
DHT dht(dhtDataPin, dhtType);

void setup() {
  Serial.begin(115200);

  //dacDisable(sensorPowerPin);
  
  dht.begin();

  WiFi.begin(ssid, password);
  Serial.println("Connecting");
  while(WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to WiFi network with IP Address: ");
  Serial.println(WiFi.localIP());

  //dacDisable(sensorPowerPin);

  pinMode(sensorPowerPin, OUTPUT);
}

void loop() {
  // check that enough time has passed
  if (lastTime == 0 || ((millis() - lastTime) > timerDelay)) {
    // check connection
    if(WiFi.status()== WL_CONNECTED){
      
      WiFiClient client;
      HTTPClient http;
      http.begin(client, apiAddress);
      http.addHeader("Content-Type", "application/json");

      String content = getJsonContent();
      int httpResponseCode = http.POST(content);
     
      Serial.print("HTTP Response code: ");
      Serial.println(httpResponseCode);

        
      // Free resources
      http.end();
    }
    else {
      Serial.println("WiFi Disconnected");
    }
    lastTime = millis();
  }
}

String getJsonContent() {
  digitalWrite(sensorPowerPin, HIGH);
  // delay set high enough for DHT sensor to spin up
  delay(1000);

  float batterySensorValue = analogRead(batteryPin);
  batterySensorValue = batterySensorValue * sensivity;
  // Calculate the percentage level
  float percentage = (batterySensorValue / maxVoltage) * 100.0;

  Serial.println("GetJsonContent");

  // read DHT values
  // temp as degrees C
  float tempValue = dht.readTemperature();
  float humidityValue = dht.readHumidity();

  if (isnan(tempValue) || isnan(humidityValue)) {
    Serial.println("Failed to read from DHT sensor");
  }

  delay(250);
  digitalWrite(sensorPowerPin, LOW);

  JSONVar sensorData;
  sensorData["deviceId"] = (int) deviceID;

  // somehow define array and populate it with jsonVar[]
  JSONVar sensorReadingsArray;

  // battery reading
  JSONVar sensorReading0;
  sensorReading0["sensorId"] = (int) 1;
  sensorReading0["sensorType"] = (int) 4;
  //sensorReading0["valueInt"] = moistureValue;
  //sensorReading0["valueBool"] = digitalVal == LOW;
  //sensorReading0["valueString"] = "";
  sensorReading0["valueFloat"] = percentage;

  sensorReadingsArray[0] = sensorReading0;

  // dht sensor readings
  // temperature
  JSONVar sensorReading1;
  sensorReading1["sensorId"] = (int) 2;
  sensorReading1["sensorType"] = (int) 2;
  sensorReading1["valueFloat"] = tempValue;

  sensorReadingsArray[1] = sensorReading1;

  // humidity
  JSONVar sensorReading2;
  sensorReading2["sensorId"] = (int) 2;
  sensorReading2["sensorType"] = (int) 3;
  sensorReading2["valueFloat"] = humidityValue;

  sensorReadingsArray[2] = sensorReading2;

  sensorData["sensorReadings"] = sensorReadingsArray;

  String jsonString = JSON.stringify(sensorData);
  Serial.println(jsonString);
  return jsonString;
}

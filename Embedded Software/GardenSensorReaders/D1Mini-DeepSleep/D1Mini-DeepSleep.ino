/*
  Ant290
  https://github.com/ant290/Samwise
*/

#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>
#include <DHT.h>

#define uS_TO_S_FACTOR 1000000  /* Conversion factor for micro seconds to seconds */
#define TIME_TO_SLEEP  3600      /* Time ESP32 will go to sleep (in seconds) */

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
const int digitalMoisturePin = 13;

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
  
  //delay may be needed if deep sleep waking stops working
  delay(500);
  Serial.println("Waking up - Setup()");

  pinMode(sensorPowerPin, OUTPUT);
  pinMode(digitalMoisturePin, INPUT);

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

  Serial.println("setup readings...");
  submitSensorReadings();
 
  Serial.println("Sleep timer set to 10 seconds (timerDelay variable).");

  Serial.println("Going to Sleep! ZzZz");
  esp_sleep_enable_timer_wakeup(TIME_TO_SLEEP * uS_TO_S_FACTOR);
  esp_deep_sleep_start();
}

void loop() {
  
}

void submitSensorReadings() {
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
}

String getJsonContent() {
  digitalWrite(sensorPowerPin, HIGH);
  // delay set high enough for DHT sensor to spin up
  delay(1000);
  delay(1000);

  float batterySensorValue = readBatteryVoltage();
  float percentage = getBatteryPercent(batterySensorValue);
  // batterySensorValue = batterySensorValue * sensivity;
  // // Calculate the percentage level
  // float percentage = (batterySensorValue / maxVoltage) * 100.0;

  Serial.println("GetJsonContent");

  // read DHT values
  // temp as degrees C
  float tempValue = dht.readTemperature();
  float humidityValue = dht.readHumidity();

  if (isnan(tempValue) || isnan(humidityValue)) {
    Serial.println("Failed to read from DHT sensor");
  }

  //read moisture pins
  int moistureValue = analogRead(analogMoisturePin);
  Serial.print("moisture: ");
  Serial.println(moistureValue);

  int digitalVal = digitalRead(digitalMoisturePin);
  Serial.print("digital moisture: ");
  Serial.println(digitalVal);

  if (digitalVal == LOW) {
    Serial.println("do not water.");
  } else {
    Serial.println("need to water!");
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
  sensorReading2["sensorId"] = (int) 3;
  sensorReading2["sensorType"] = (int) 3;
  sensorReading2["valueFloat"] = humidityValue;

  sensorReadingsArray[2] = sensorReading2;

  // soil moisture reading
  JSONVar sensorReading3;
  sensorReading3["sensorId"] = (int) 4;
  sensorReading3["sensorType"] = (int) 1;
  sensorReading3["valueInt"] = moistureValue;
  sensorReading3["valueBool"] = digitalVal == LOW;

  sensorReadingsArray[3] = sensorReading3;

  sensorData["sensorReadings"] = sensorReadingsArray;

  String jsonString = JSON.stringify(sensorData);
  Serial.println(jsonString);
  return jsonString;
}

float readBatteryVoltage() {
  float R1 = 5100.0;
  float R2 = 10000.0;

  int raw = analogRead(batteryPin);

  float pinVoltage = raw * (3.3 / 4095.0);
  Serial.print("pin voltage: ");
  Serial.println(pinVoltage);
  
  float battVoltage = pinVoltage * (R1 + R2) / R2;
  //float batteryVoltage = pinVoltage / R2 * (R1 + R2);
  Serial.print("battery voltage: ");
  Serial.println(battVoltage);

  return battVoltage;
}

float getBatteryPercent(float batteryVoltage) {
  float vmin = 3.0;
  float vmax = 4.2;

  if (batteryVoltage < vmin) batteryVoltage = vmin;
  if (batteryVoltage > vmax) batteryVoltage = vmax;
  float percentage = (batteryVoltage - vmin) / (vmax -vmin) * 100.0;
  return percentage;
}

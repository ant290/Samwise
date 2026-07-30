/*
  Ant290
  https://github.com/ant290/Samwise
*/

#include <WiFi.h>
#include <HTTPClient.h>
#include <Arduino_JSON.h>

const int deviceID = 1;
const char* ssid = "NETWORK_NAME";
const char* password = "NETWORK_PASSWORD";

//Your Domain name with URL path or IP address with path
const char* apiAddress = "http://LOCAL_IP:5010/api/gardensensor";

//AO pin on moisture reader
const int analogMoisturePin = 34;

//DO pin on moisture reader
const int digitalMoisturePin = 35;

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
unsigned long lastTime = 0;
// Timer set to 15 minutes (900000)
unsigned long timerDelay = 900000;
// Set timer to 5 seconds (5000)
//unsigned long timerDelay = 5000;

void setup() {
  Serial.begin(115200);

  pinMode(digitalMoisturePin, INPUT);

  WiFi.begin(ssid, password);
  Serial.println("Connecting");
  while(WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to WiFi network with IP Address: ");
  Serial.println(WiFi.localIP());
 
  Serial.println("Timer set to 5 seconds (timerDelay variable), it will take 5 seconds before publishing the first reading.");
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

  JSONVar sensorData;
  sensorData["deviceId"] = (int) deviceID;
  // jsonObject["soilReading"] = "value";

  // somehow define array and populate it with jsonVar[]
  JSONVar sensorReadingsArray;

  JSONVar sensorReading0;
  sensorReading0["sensorId"] = (int) 1;
  sensorReading0["sensorType"] = (int) 1;
  sensorReading0["valueInt"] = moistureValue;
  sensorReading0["valueBool"] = digitalVal == LOW;
  sensorReading0["valueString"] = "";

  sensorReadingsArray[0] = sensorReading0;

  sensorData["sensorReadings"] = sensorReadingsArray;

  String jsonString = JSON.stringify(sensorData);
  Serial.println(jsonString);
  return jsonString;
}
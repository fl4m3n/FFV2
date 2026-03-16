//###################################################################################################################
//###################################################################################################################
// Extrality Lab 2026 - Stockholm University
// Questions? Contact antonio.braga@dsv.su.se or luis.quintero@dsv.su.se
//###################################################################################################################
// This file should be ran together with the Python server file and one of the ESP32 provided to you at Extrality Lab
// ###################################################################################################################
// To run this file, you need to have the 'Websockets' by Markus Sattler and "Adafruit Neopixel" library installed.
// ###################################################################################################################
// ###################################################################################################################


#include <WiFi.h>
#include <WebSocketsClient.h>
#include <Adafruit_NeoPixel.h>
#include <header.h>

// --------------------------------------------------
// WiFi / Server Settings
// --------------------------------------------------
const char *ssid = "dsv-extrality-lab";            // Enter your Wi-Fi name
const char *password = "expiring-unstuck-slider";  // Enter Wi-Fi password
const char* serverIP = "10.204.0.28"; //Replace with your Python server's IP (e.g. 192.168.1.111)
const uint16_t serverPort = 8081; //Replace with your desired Port (or keep as is)

// --------------------------------------------------
// Hardware pins
// --------------------------------------------------

// SPARKFUN ESP32 Thing plus
#define BUILTIN_BUTTON_PIN 0
// #define GYRO 25
#define BUTTON_RESTART 4

#define xP 26
#define yP 25


// --------------------------------------------------
// WebSocket events setup
// --------------------------------------------------
void webSocketEvent(WStype_t type, uint8_t* payload, size_t length) {

  switch(type) {

  
    case WStype_CONNECTED: {
      Serial.printf("WebSocket connected to %s:%u\n", serverIP, serverPort);
      String message = String("Device: ") + BOARD_NAME + " ... MAC: " + WiFi.macAddress();
      webSocket.sendTXT(message);
      break;
    }

    case WStype_DISCONNECTED:
      Serial.println("Not connected to WebSocket server... Retrying in 5 seconds");
      break;

    case WStype_TEXT:
      payload[length] = '\0';
      Serial.print("WebSocket received: ");
      Serial.println((char*)payload);
      handleMessage(String((char*)payload));
      break;
  }
}

// --------------------------------------------------
// WebSocket message handler. Edit for new RECEIVED WebSocket Messages
// --------------------------------------------------
void handleMessage(const String& message) { // This function is set up to receive and parse messages in the form of "TYPE:VALUE" (e.g. Led:55)
  if (message == "reset") {
    Serial.println("Reset command received");
    ESP.restart();   // for ESP32 / ESP8266
  }

  if(message == ("ping")) {
    Serial.println("pong");
    webSocket.sendTXT("pong");
  }

  int sep = message.indexOf(':');
  if (sep == -1) return;

  String type = message.substring(0, sep);
  int value = message.substring(sep + 1).toInt();

  if(type.equalsIgnoreCase("CUSTOM WEBSOCKET MESSAGE")) {
    //Do something
  }

}

// --------------------------------------------------
// Setup
// --------------------------------------------------
void setup() {
  Serial.begin(115200);
  delay(100);

  Serial.println();
  Serial.println("=================================");
  Serial.print("Compiled for board: ");
  Serial.println(BOARD_NAME);
  Serial.println("=================================");

  pinMode(BUILTIN_BUTTON_PIN, INPUT_PULLUP);

  // pinMode(BUTTON_0, INPUT_PULLUP);
  // pinMode(BUTTON_1, INPUT_PULLUP);
  // pinMode(BUTTON_2, INPUT_PULLUP);
  // pinMode(BUTTON_3, INPUT_PULLUP);

  //pinMode(GYRO, INPUT);
  pinMode(BUTTON_RESTART, INPUT_PULLUP);

  pinMode(xP, INPUT);
  pinMode(yP, INPUT);

  // pinMode(LAMP_PIN, OUTPUT);
  // digitalWrite(LAMP_PIN, LOW);  // Start OFF

  #ifdef LED_TYPE_GPIO //If ESP32 regular or S2
    ledcAttach(LED_PIN, LEDC_FREQUENCY, LEDC_RESOLUTION);
    ledcWrite(LED_PIN, 0);
  #endif

  #ifdef LED_TYPE_RGB //If ESP32 S3
    rgbLed.begin();
    rgbLed.clear();
    rgbLed.show();
  #endif

  WiFi.begin(ssid, password);
  Serial.printf("Connecting to WiFi: %s", ssid);
  while (WiFi.status() != WL_CONNECTED) { //Waiting for conection to wifi
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected");
  Serial.printf("Attempting to connect to WebSocket Server on %s\n", serverIP);

  webSocket.begin(serverIP, serverPort, "/");
  webSocket.onEvent(webSocketEvent);
  webSocket.setReconnectInterval(5000);


  // Set initial values of 
}

// --------------------------------------------------
// Loop
// --------------------------------------------------

void loop() {
  webSocket.loop();

  // Gyroscope readings
  int px, py;
  int ax, ay;

  px = pulseIn(xP, HIGH);
  py = pulseIn(yP, HIGH);

  ax = ((px / 10) - 500) * 8;
  ay = ((py / 10) - 500) * 8;

  // Check what interval ax & ay are in
  String axStatusCurrent = checkInterval(ax);
  String ayStatusCurrent = checkInterval(ay);

  // If the status is different from before, send the data
  // fl = far left, sl = slightly left, ne = neutral, sr = slightly right, fr = far right
  if (axStatusCurrent != axStatusPrevious) {
    axStatusPrevious = axStatusCurrent;
    Serial.println("ax:" + axStatusCurrent);
    webSocket.sendTXT("ax:" + axStatusCurrent);
  }
  if (ayStatusCurrent != ayStatusPrevious) {
    ayStatusPrevious = ayStatusCurrent;
    Serial.println("ay:" + ayStatusCurrent);
    webSocket.sendTXT("ay:" + ayStatusCurrent);
  }

  // Builtin Button readings
  bool currentButtonState = digitalRead(BUILTIN_BUTTON_PIN);

  if (lastButtonState == HIGH && currentButtonState == LOW) { //Sends Websocket Message when you push down the built in button
    webSocket.sendTXT("button:1");
    Serial.println("Button Down");
  }

  if (lastButtonState == LOW && currentButtonState == HIGH) { //Sends Websocket Message when you stop pushing the built in button
    webSocket.sendTXT("button:0");
    Serial.println("Button Up");
  }
  lastButtonState = currentButtonState;


  // Restart Button readings
  bool currentButtonRestartState = digitalRead(BUTTON_RESTART);

  if (lastButtonRestartState == HIGH && currentButtonRestartState == LOW) { //Sends Websocket Message when you push down the Restart button
    webSocket.sendTXT("buttonR:1");
    Serial.println("Button_Restart Down");
  }

  if (lastButtonRestartState == LOW && currentButtonRestartState == HIGH) { //Sends Websocket Message when you stop pushing the Restart button
    webSocket.sendTXT("buttonR:0");
    Serial.println("Button_Restart Up");
  }
  lastButtonRestartState = currentButtonRestartState;

  delay(100);
}

// --------------------------------------------------
// Helper Functions
// --------------------------------------------------

String checkInterval(int gyroValue){
  if (gyroValue < farLeft) {
    return "fl";
  }
  else if (gyroValue > farLeft && gyroValue < slightlyLeft) {
    return "sl";
  }
  else if (gyroValue > slightlyLeft && gyroValue < slightlyRight) {
    return "ne";
  }
  else if (gyroValue > slightlyRight && gyroValue < farRight) {
    return "sr";
  }
  else if (gyroValue > farRight) {
    return "fr";
  }
  return "ne";
}
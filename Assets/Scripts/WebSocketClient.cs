using UnityEngine;
using NativeWebSocket;
using UnityEngine.Events;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class WebSocketClientExample : MonoBehaviour
{
    private WebSocket websocket;
    public string serverIP = "10.204.0.28"; // Replace with your server's IP address
    public int serverPort = 8081; // Replace with your server's port number (8081 is the default)
    
    public MoveHandler moveHandler;
    public RestartHandler restartHandler;
    public ParachuteHandler parachuteHandler;

    async void Start()
    {
        websocket = new WebSocket("ws://" + serverIP + ":" + serverPort);

        //Runs when connected to the server
        websocket.OnOpen += async () =>
        {
            Debug.Log("Connected to WebSocket server");
            string UUID = SystemInfo.deviceUniqueIdentifier; // Certain devices block MAC address access for privacy reasons so we send a UUID instead

            await websocket.SendText("Device (Unity):" + SystemInfo.deviceName + " ... Device's Unique Identifier: " + UUID);
        };

        //Runs when a message is received from the server
        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);

            IncomingMessageParser(message);

        };

        //Runs when disconnected from the server
        websocket.OnClose += (code) =>
        {
            Debug.Log("WebSocket closed");
        };

        await websocket.Connect();
    }

    void Update()
    {
        //Although not necessary for our lab, I have left this here as a reference
        //Websockets will not work on WebGL builds so with this preprocessor directive we include all builds except WebGL as well as including the editor for testing purposes
        #if !UNITY_WEBGL || UNITY_EDITOR 

            websocket.DispatchMessageQueue();
        #endif
    }

    async void OnDestroy()
    {
        //if (websocket != null)
            //await websocket.Close();
    }

    public async void SendHello()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("Hello from Unity");
            Debug.Log("Sent: Hello from Unity");
        }
        else
        {
            Debug.LogWarning("WebSocket not connected");
        }
    }

    public void IncomingMessageParser(string msg)
    {
        if (msg.IndexOf(":") == -1){
            return;
        }

        // Example messages: "ax:ne" / "buttonR:1"
        string messageType = msg.Substring(0, msg.IndexOf(":"));
        string messageValue = msg.Substring(msg.IndexOf(":") + 1).Trim();

        // Send to MoveHandler if incoming message is from the gyroscope
        if (messageType == "ax" || messageType == "ay"){
            if (moveHandler != null)
            {
                moveHandler.ReceiveMovementMessage(messageType, messageValue);
            }
        }

        // Send to RestartHandler if incoming message is from restart button
        if (messageType == "buttonR"){
            if (restartHandler != null){
                // Convert value to int (0 or 1)
                restartHandler.ReceiveMessage(messageType, int.Parse(messageValue));
            }
            
        }
        
        // Send to parachuteHandler if incoming message is from parachute button
        if (messageType == "buttonP"){
            if (parachuteHandler != null){
                // Convert value to int (0 or 1)
                parachuteHandler.ReceiveMessage(messageType, int.Parse(messageValue));
            }
        }
        
        
    }

}


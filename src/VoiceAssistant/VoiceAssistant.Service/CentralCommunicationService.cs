// /*
//  * Class: CentralCommunicationService.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using VoiceAssistant.Communication.Protocols;

namespace VoiceAssistant.Service;

public class CentralCommunicationService:IDisposable
{
    public readonly WebSocketProtocol WebSocketProtocol;
    public readonly SerialPortProtocol SerialPortProtocol;
    public CentralCommunicationService()
    {
        // SerialPortProtocol = new SerialPortProtocol("");
        // WebSocketProtocol = new WebSocketProtocol("ws://192.168.20.67:8080/ws");
        WebSocketProtocol = new WebSocketProtocol("ws://192.168.20.121:8080/ws");
        // InitSerialProtocol();
        InitWebsocketProtocol();
        
    }

    private void InitSerialProtocol()
    {
        SerialPortProtocol.Connect();
    }

    private void InitWebsocketProtocol()
    {
        _ = WebSocketProtocol.ConnectAsync();
        
    }
    
    public void Dispose()
    {
        WebSocketProtocol.Dispose();
    }
}
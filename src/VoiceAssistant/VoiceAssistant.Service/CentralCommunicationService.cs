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

    public CentralCommunicationService()
    {
        WebSocketProtocol = new WebSocketProtocol("ws://192.168.246.1:8080/ws");
        _ = WebSocketProtocol.ConnectAsync();
    }

    public void Dispose()
    {
        WebSocketProtocol.Dispose();
    }
}
// /*
//  * Class: SentMessageInfo.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

namespace VoiceAssistant.Service.Entity;

public class SentMessageInfo
{
    public string Id { get; set; }
    
    public long Timestamp { get; set; }
    
    public string PayloadType { get; set; }
    
    public string JsonContent { get; set; }
    
    public MessageStatus Status { get; set; }
    
    public string? ResponseJson { get; set; }
    
    public long? ResponseTime { get; set; }
}
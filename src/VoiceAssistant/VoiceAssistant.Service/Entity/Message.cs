/*
 * Class: MessagePayload.cs
 * Author: lijianing
 * Date: 2025-12-12
 * Description: 
 * Version: 1.0
 */
using Newtonsoft.Json;
namespace VoiceAssistant.Service.Entity;

public class Message<TPayload>
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("ts")]
    public long Timestamp { get; set; }
    
    [JsonProperty("payload")]
    public TPayload Payload { get; set; }
}
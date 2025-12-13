// /*
//  * Class: CheckInCommand.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;
using VoiceAssistant.Service.Entity.Payloads;

namespace VoiceAssistant.Service.Entity.Commands;

public class CheckInCommand
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("ts")]
    public long Timestamp { get; set; }

    [JsonProperty("payload")]
    public CheckInPayload Payload { get; set; }
}
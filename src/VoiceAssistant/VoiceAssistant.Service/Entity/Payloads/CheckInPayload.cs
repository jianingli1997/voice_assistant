// /*
//  * Class: CheckInPayload.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;

namespace VoiceAssistant.Service.Entity.Payloads;

public class CheckInPayload
{
    [JsonProperty("device_type")]
    public string DeviceType { get; set; }
    
    [JsonProperty("version")]
    public string Version { get; set; } = "VA1.0.0.0";
}
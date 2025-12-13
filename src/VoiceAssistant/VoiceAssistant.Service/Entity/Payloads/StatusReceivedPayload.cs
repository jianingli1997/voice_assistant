// /*
//  * Class: StatusReceivedPayload.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;

namespace VoiceAssistant.Service.Entity.Payloads;

public class StatusReceivedPayload: ReceivedPayloadBase
{
    [JsonProperty("time_status")]
    public long Time { get; set; }
    
    
    /// <summary>
    /// 心跳间隔
    /// </summary>
    [JsonProperty("hb_interval")]
    public int HeartbeatInterval { get; set; }
    
    
    /// <summary>
    /// 超时次数
    /// </summary>
    [JsonProperty("hb_times")]
    public int HeartbeatTimes { get; set; }
}
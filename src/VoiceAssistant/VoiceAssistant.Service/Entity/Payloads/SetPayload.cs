// /*
//  * Class: SetPayload.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;
using VoiceAssistant.Service.Entity.Commands;

namespace VoiceAssistant.Service.Entity.Payloads;

public class SetPayload:SendPayloadBase
{
    [JsonProperty("cmd")]
    public SetCommand Command { get; set; }
}
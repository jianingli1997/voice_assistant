// /*
//  * Class: ResponseReceivedPayload.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;
using VoiceAssistant.Service.Utils;

namespace VoiceAssistant.Service.Entity.Payloads;

public class ResponseReceivedPayload:ReceivedPayloadBase
{
    [JsonProperty("ori_id")]
    public string OriginalId { get; set; }

    [JsonProperty("cmd")]
    public string Command { get; set; }

    [JsonProperty("msg")]
    public string Message { get; set; }

    [JsonProperty("cur_val")]
    public int CurrentValue { get; set; }

    [JsonProperty("ret_code")]
    public int ReturnCode { get; set; }

    [JsonIgnore]
    public Enum CommandEnum
    {
        get => EnumMemberMapper.GetEnum(this.Command);
        set => this.Command = EnumMemberMapper.GetString(value);
    }

    [JsonIgnore]
    public string CommandCategory => CommandEnum.GetType().Name.Replace("Command", "");
}
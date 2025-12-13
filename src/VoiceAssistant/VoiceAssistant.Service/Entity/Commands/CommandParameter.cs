// /*
//  * Class: CommandParameter.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;

namespace VoiceAssistant.Service.Entity.Commands;

public class CommandParameter
{
    [JsonProperty("val")]
    public object Value { get; set; }

    [JsonProperty("min")]
    public object Min { get; set; }

    [JsonProperty("max")]
    public object Max { get; set; }
}
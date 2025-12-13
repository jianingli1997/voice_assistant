// /*
//  * Class: PayloadJsonConverter.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VoiceAssistant.Service.Entity.Payloads;

namespace VoiceAssistant.Service.Utils;

public class PayloadJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ReceivedPayloadBase);
    }

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        // 心跳 / 状态上报
        if (jObject["hb_interval"] != null &&
            jObject["hb_times"] != null)
        {
            return jObject.ToObject<StatusReceivedPayload>();
        }


        // 普通 EIP 响应
        return jObject.ToObject<ResponseReceivedPayload>();
    }

    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        JObject.FromObject(value).WriteTo(writer);
    }
}
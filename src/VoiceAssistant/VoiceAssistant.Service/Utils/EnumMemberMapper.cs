// /*
//  * Class: EnumMemberMapper.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;
using VoiceAssistant.Service.Entity.Commands;

namespace VoiceAssistant.Service.Utils;

public static class EnumMemberMapper
{
    private static readonly ConcurrentDictionary<string, Enum?> StringToEnum =
        new ConcurrentDictionary<string, Enum?>(StringComparer.OrdinalIgnoreCase);

    private static readonly ConcurrentDictionary<Enum, string> EnumToString =
        new ConcurrentDictionary<Enum, string>();

    static EnumMemberMapper()
    {
        RegisterEnum<SetCommand>();
        RegisterEnum<GetCommand>();
    }

    public static void RegisterEnum<TEnum>() where TEnum : Enum
    {
        foreach (FieldInfo field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            Enum enumValue = (Enum)field.GetValue(null)!;
            string enumString =
                field.GetCustomAttribute<EnumMemberAttribute>()?.Value
                ?? field.Name;

            StringToEnum[enumString] = enumValue;
            EnumToString[enumValue] = enumString;
        }
    }

    public static Enum GetEnum(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        StringToEnum.TryGetValue(s, out var value);
        return value;
    }

    public static string GetString(Enum value)
    {
        if (value == null)
            return null;

        return EnumToString.TryGetValue(value, out var str)
            ? str
            : value.ToString();
    }
}



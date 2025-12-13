// /*
//  * Class: SetCommand.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VoiceAssistant.Service.Entity.Commands;

[JsonConverter(typeof(StringEnumConverter))]
public enum SetCommand
{
    [EnumMember(Value = "eip_set_Snap")] Snap,

    [EnumMember(Value = "eip_set_Record")] Record,

    [EnumMember(Value = "eip_set_Brightness")]
    Brightness,

    [EnumMember(Value = "eip_set_Contrast")]
    Contrast,

    [EnumMember(Value = "eip_set_Rotation")]
    Rotation,

    [EnumMember(Value = "eip_set_DigitalZoom")]
    DigitalZoom,

    [EnumMember(Value = "eip_set_Hue")] Hue,

    [EnumMember(Value = "eip_set_Dae")] Dae,

    [EnumMember(Value = "eip_set_Defog")] Defog,

    [EnumMember(Value = "eip_set_Ars")] Ars,

    [EnumMember(Value = "eip_set_FluoSwitch")]
    FluoSwitch,

    [EnumMember(Value = "eip_set_FluoBrightness")]
    FluoBrightness,

    [EnumMember(Value = "eip_set_FluoGain")]
    FluoGain,

    [EnumMember(Value = "eip_set_FluoColour")]
    FluoColour,

    [EnumMember(Value = "eip_set_PipSwitch")]
    PipSwitch,

    [EnumMember(Value = "eip_set_msgetCfg")]
    MsgetCfg,

    [EnumMember(Value = "usp_set_MinEnergy")]
    UspMinEnergy,

    [EnumMember(Value = "ins_set_FlowMode")]
    InsFlowMode,

    [EnumMember(Value = "ins_set_Pressure")]
    InsPressure,

    [EnumMember(Value = "ins_set_FlowValue")]
    InsFlowValue,

    [EnumMember(Value = "ins_set_GasHeat")]
    InsGasHeat,

    [EnumMember(Value = "ep_set_MonoCutMode")]
    EpMonoCutMode,

    [EnumMember(Value = "ep_set_MonoCogaMode")]
    EpMonoCogaMode,

    [EnumMember(Value = "ep_set_BipCogaMode")]
    EpBipCogaMode,

    [EnumMember(Value = "ep_set_MonoCutPower")]
    EpMonoCutPower,

    [EnumMember(Value = "ep_set_MonoCogaPower")]
    EpMonoCogaPower,

    [EnumMember(Value = "ep_set_BipCogaPower")]
    EpBipCogaPower,

    [EnumMember(Value = "va_set_Switch")] VoiceAssistant,

    [EnumMember(Value = "ai_set_OpenBleed")]
    AiOpenBleed,

    [EnumMember(Value = "ai_set_Openseg")] AiOpenSeg
}
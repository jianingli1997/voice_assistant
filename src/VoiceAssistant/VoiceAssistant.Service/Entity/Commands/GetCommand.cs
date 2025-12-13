// /*
//  * Class: GetCommand.cs
//  * Author: lijianing
//  * Date: 2025-12-12
//  * Description: 
//  * Version: 1.0
//  */

using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VoiceAssistant.Service.Entity.Commands;

[JsonConverter(typeof(StringEnumConverter))]
public enum GetCommand
{
    [EnumMember(Value = "eip_get_RecordState")]
    RecordState,

    [EnumMember(Value = "eip_get_SwitchLR")] [UsedImplicitly]
    SwitchLr,

    [EnumMember(Value = "eip_get_Brightness")]
    Brightness,

    [EnumMember(Value = "eip_get_Contrast")]
    Contrast,

    [EnumMember(Value = "eip_get_DigitalZoom")]
    DigitalZoom,

    [EnumMember(Value = "eip_get_Rotation")]
    Rotation,
    [EnumMember(Value = "eip_get_Hue")] Hue,
    [EnumMember(Value = "eip_get_Dae")] Dae,
    [EnumMember(Value = "eip_get_Defog")] Defog,
    [EnumMember(Value = "eip_get_Ars")] Ars,

    [EnumMember(Value = "eip_get_FluoSwitch")]
    FluoSwitch,

    [EnumMember(Value = "eip_get_FluoBrightness")]
    FluoBrightness,

    [EnumMember(Value = "eip_get_FluoColour")]
    FluoColour,

    [EnumMember(Value = "eip_get_FluoGain")]
    FluoGain,

    [EnumMember(Value = "eip_get_PipSwitch")]
    PipSwitch,

    [EnumMember(Value = "usp_get_MinEnergy")]
    UspMinEnergy,

    [EnumMember(Value = "ins_get_FlowMode")]
    InsFlowMode,

    [EnumMember(Value = "ins_get_Pressure")]
    InsPressure,

    [EnumMember(Value = "ins_get_FlowValue")]
    InsFlowValue,

    [EnumMember(Value = "ins_get_GasHeat")]
    InsGasHeat,

    [EnumMember(Value = "ep_get_MonoCutMode")]
    EpMonoCutMode,

    [EnumMember(Value = "ep_get_MonoCogaMode")]
    EpMonoCogaMode,

    [EnumMember(Value = "ep_get_BipCogaMode")]
    EpBipCogaMode,

    [EnumMember(Value = "ep_get_MonoCutPower")]
    EpMonoCutPower,

    [EnumMember(Value = "ep_get_MonoCogaPower")]
    EpMonoCogaPower,

    [EnumMember(Value = "ep_get_BipCogaPower")]
    EpBipCogaPower,
    [EnumMember(Value = "va_get_Switch")] 
    VoiceAssistant,

    [EnumMember(Value = "ai_get_OpenBleed")]
    AiOpenBleed,
    [EnumMember(Value = "ai_get_Openseg")] 
    AiOpenSeg,
}
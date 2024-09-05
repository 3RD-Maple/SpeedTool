using System.Numerics;
using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUISettings : IConfigurationSection
{
    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 SecondsClockTimerColor;

    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 MinutesClockTimerColor;

    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 HoursClockTimerColor;

    [JsonInclude]
    public string Theme = "Light";
}

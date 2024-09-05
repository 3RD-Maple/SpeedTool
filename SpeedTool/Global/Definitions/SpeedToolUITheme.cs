using System.Numerics;
using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUITheme
{
    [JsonConverter(typeof(Vector4Converter))]
    public Vector4 SecondsClockTimerColor { get; set; }

    [JsonConverter(typeof(Vector4Converter))]
    public Vector4 MinutesClockTimerColor { get; set; }

    [JsonConverter(typeof(Vector4Converter))]
    public Vector4 HoursClockTimerColor { get; set; }
}

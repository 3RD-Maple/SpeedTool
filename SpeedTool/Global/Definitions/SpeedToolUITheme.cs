using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUITheme
{
    public SpeedToolUITheme(JsonObject node)
    {
        SecondsClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"SecondsClockTimeColor"]!.AsObject());
        MinutesClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"MinutesClockTimeColor"]!.AsObject());
        HoursClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"HoursClockTimeColor"]!.AsObject());
    }
    public Vector4 SecondsClockTimerColor { get; set; }
    public Vector4 MinutesClockTimerColor { get; set; }
    public Vector4 HoursClockTimerColor { get; set; }
}
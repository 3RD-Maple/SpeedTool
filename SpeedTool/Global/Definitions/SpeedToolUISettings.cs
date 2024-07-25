using System.Numerics;
using System.Text.Json.Nodes;
using Silk.NET.OpenGL;
using SpeedTool.Util;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUISettings : IConfigurationSection
{
    public Vector4 SecondsClockTimerColor;
    public Vector4 MinutesClockTimerColor;
    public Vector4 HoursClockTimerColor;
    
    public void FromJSONObject(JsonObject node)
    {
        SecondsClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"SecondsClockTimeColor"]!.AsObject());
        MinutesClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"MinutesClockTimeColor"]!.AsObject());
        HoursClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"HoursClockTimeColor"]!.AsObject());
    }

    public JsonObject ToJSONObject()
    {
        var node = new JsonObject();
        node["SecondsClockTimeColor"] = SecondsClockTimerColor.ToJsonObject();
        node["MinutesClockTimeColor"] = MinutesClockTimerColor.ToJsonObject();
        node["HoursClockTimeColor"] = HoursClockTimerColor.ToJsonObject();

        return node;
    }
}
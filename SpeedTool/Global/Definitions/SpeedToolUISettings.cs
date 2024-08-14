using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUISettings : IConfigurationSection
{
    public Vector4 SecondsClockTimerColor;
    public Vector4 MinutesClockTimerColor;
    public Vector4 HoursClockTimerColor;

    public string Theme { get; set; } = "Light";

    public void FromJSONObject(JsonObject node)
    {
        SecondsClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"SecondsClockTimeColor"]!.AsObject());
        MinutesClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"MinutesClockTimeColor"]!.AsObject());
        HoursClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"HoursClockTimeColor"]!.AsObject());
        Theme = (string)node["Theme"]!.AsValue()!;
    }

    public JsonObject ToJSONObject()
    {
        var node = new JsonObject();
        node["SecondsClockTimeColor"] = SecondsClockTimerColor.ToJsonObject();
        node["MinutesClockTimeColor"] = MinutesClockTimerColor.ToJsonObject();
        node["HoursClockTimeColor"] = HoursClockTimerColor.ToJsonObject();

        node["Theme"]= Theme;

        return node;
    }
}

using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Util;
using SpeedTool.Platform;

namespace SpeedTool.Global.Definitions;

public sealed class SpeedToolUISettings : IConfigurationSection
{
    public Vector4 SecondsClockTimerColor;
    
    public Vector4 MinutesClockTimerColor;
    
    public Vector4 HoursClockTimerColor;
    
    public Hotkey StartHotkey;
    
    public Hotkey PauseHotkey;
    
    public Hotkey ResetHotkey;

    public string Theme { get; set; } = "Light";
    
    
    public void FromJSONObject(JsonObject node)
    {
        SecondsClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"SecondsClockTimeColor"]!.AsObject());
        MinutesClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"MinutesClockTimeColor"]!.AsObject());
        HoursClockTimerColor = JSONHelper.Vector4FromJsonObject(node[$"HoursClockTimeColor"]!.AsObject());
        
        StartHotkey = Hotkey.FromJSONObject(node["StartHotkey"]!.AsObject());
        PauseHotkey = Hotkey.FromJSONObject(node["PauseHotkey"]!.AsObject());
        ResetHotkey = Hotkey.FromJSONObject(node["ResetHotkey"]!.AsObject());

        Theme = (string)node["Theme"]!.AsValue()!;
    }

    public JsonObject ToJSONObject()
    {
        var node = new JsonObject();
        node["SecondsClockTimeColor"] = SecondsClockTimerColor.ToJsonObject();
        node["MinutesClockTimeColor"] = MinutesClockTimerColor.ToJsonObject();
        node["HoursClockTimeColor"] = HoursClockTimerColor.ToJsonObject();
        
        node["StartHotkey"] = Hotkey.ToJSONObject(StartHotkey);
        node["PauseHotkey"] = Hotkey.ToJSONObject(PauseHotkey);
        node["ResetHotkey"] = Hotkey.ToJSONObject(ResetHotkey);

        node["Theme"]= Theme;
        
        return node;
    }
}

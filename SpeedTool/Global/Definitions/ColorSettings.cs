using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Global.Definitions;

public sealed class ColorSettings : IConfigurationSection
{
    public Vector4 TextColor;
    public Vector4 AheadColor;
    public Vector4 BehindColor;
    public Vector4 PBColor;
    
    public void FromJSONObject(JsonObject node)
    {
        TextColor = JSONHelper.Vector4FromJsonObject(node[$"TextColor"]!.AsObject());
        AheadColor = JSONHelper.Vector4FromJsonObject(node[$"AheadColor"]!.AsObject());
        BehindColor = JSONHelper.Vector4FromJsonObject(node[$"BehindColor"]!.AsObject());
        PBColor = JSONHelper.Vector4FromJsonObject(node[$"PBColor"]!.AsObject());
    }

    public JsonObject ToJSONObject()
    {
        var node = new JsonObject();
        node["TextColor"] = TextColor.ToJsonObject();
        node["AheadColor"] = AheadColor.ToJsonObject();
        node["BehindColor"] = BehindColor.ToJsonObject();
        node["PBColor"] = PBColor.ToJsonObject();
        
        return node;
    }
}
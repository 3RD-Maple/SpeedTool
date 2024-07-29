using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Global.Definitions;

public sealed class ClassicUISettings : IConfigurationSection
{
    public Vector4 ActiveSplitColor;
    public int ShownSplitsCount;
    
    public void FromJSONObject(JsonObject node)
    {
        ActiveSplitColor = JSONHelper.Vector4FromJsonObject(node[$"ActiveSplitColor"]!.AsObject());
        ShownSplitsCount = (int)node["ShownSplitsCount"]!;
    }

    public JsonObject ToJSONObject()
    {
        var node = new JsonObject();
        node["ActiveSplitColor"] = ActiveSplitColor.ToJsonObject();
        node["ShownSplitsCount"] = ShownSplitsCount;
        
        return node;
    }
}
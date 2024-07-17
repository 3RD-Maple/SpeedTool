using System.Text.Json.Nodes;

namespace SpeedTool.Global;

public interface IConfigurationSection
{
    JsonObject ToJSONObject();
    void FromJSONObject(JsonObject node);
}

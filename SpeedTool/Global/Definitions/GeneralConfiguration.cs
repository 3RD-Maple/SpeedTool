using SpeedTool.Util;
using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Global.Definitions
{
    /// <summary>
    /// Basic configuration file (mostly as template)
    /// </summary>
    public class GeneralConfiguration : IConfigurationSection
    {
        /// <summary>
        /// Color for all system text fiels
        /// </summary>
        public Vector4 TextColor { get; set; }

        public void FromJSONObject(JsonObject node)
        {
            TextColor = JSONHelper.Vector4FromJsonObject(node["TextColor"]!.AsObject());
        }

        public JsonObject ToJSONObject()
        {
            var node = new JsonObject();
            node["TextColor"] = TextColor.ToJsonObject();
            return node;
        }
    }
}

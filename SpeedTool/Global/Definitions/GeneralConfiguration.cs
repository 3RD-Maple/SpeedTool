using SpeedTool.Util;
using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Global.Definitions
{
    /// <summary>
    /// Basic configuration file (mostly as template)
    /// </summary>
    public sealed class GeneralConfiguration : IConfigurationSection
    {
        /// <summary>
        /// Color for all system text fiels
        /// </summary>
        public Vector4 TextColor { get; set; }

        public ColorSettings? ColorConfig { get; set; } =
            Configuration.GetSection<ColorSettings>();
        
        public SpeedToolUISettings? SpeedToolUIConfig { get; set; } =
            Configuration.GetSection<SpeedToolUISettings>();
        
        public ClassicUISettings? ClassicUISettings { get; set; } =
            Configuration.GetSection<ClassicUISettings>();

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

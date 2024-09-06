using SpeedTool.JSON;
using System.Numerics;
using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(Vector4Converter))]
        public Vector4 TextColor { get; set; }

        public string TimerUI { get; set; } = "SpeedTool";
    }
}

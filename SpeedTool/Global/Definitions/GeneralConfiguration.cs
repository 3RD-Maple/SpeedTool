using SpeedTool.Global.Json;
using System.Numerics;
using System.Text.Json.Serialization;

namespace SpeedTool.Global.Definitions
{
    /// <summary>
    /// Basic configuration file (mostly as template)
    /// </summary>
    public class GeneralConfiguration
    {
        /// <summary>
        /// Color for all system text fiels
        /// </summary>
        [JsonConverter(typeof(ColorVector4Converter))]
        public Vector4 TextColor { get; set; }
    }
}

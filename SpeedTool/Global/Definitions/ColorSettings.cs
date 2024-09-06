using System.Numerics;
using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Global.Definitions;

public sealed class ColorSettings : IConfigurationSection
{
    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 TextColor;

    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 AheadColor;

    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 BehindColor;

    [JsonConverter(typeof(Vector4Converter))]
    [JsonInclude]
    public Vector4 PBColor;
}

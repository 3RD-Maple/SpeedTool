using System.Numerics;
using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Global.Definitions;

public sealed class ClassicUISettings : IConfigurationSection
{
    [JsonInclude]
    [JsonConverter(typeof(Vector4Converter))]
    public Vector4 ActiveSplitColor;

    [JsonInclude]
    public int ShownSplitsCount = 7;

    [JsonInclude]
    public bool ShowRTA = true;

    [JsonInclude]
    public bool AlternateSplitBackround = true;
}

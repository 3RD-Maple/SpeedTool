using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Splits;

public struct SplitInfo
{
    public SplitInfo() { }

    [JsonInclude]
    public string Name = "";

    [JsonInclude]
    public int Level = 0;

    /// <summary>
    /// Time spent in this segment
    /// </summary>
    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection SegmentTime = new();

    /// <summary>
    /// Difference from previous run
    /// </summary>
    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection DeltaTime = new();

    /// <summary>
    /// Total time spent to arrive at that split
    /// </summary>
    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection TotalTime = new();
}

using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Splits;

public class SplitDisplayInfo
{
    public SplitDisplayInfo(string name, bool active, int level, bool leaf)
    {
        DisplayString = name;
        IsCurrent = active;
        Level = level;
        IsLeaf = leaf;
    }

    public SplitInfo ToSplitInfo()
    {
        return new SplitInfo()
        {
            DeltaTime = DeltaTimes,
            Name = DisplayString,
            SegmentTime = SegmentTimes,
            TotalTime = Times,
            Level = Level
        };
    }

    /// <summary>
    /// Is this a split that's currently being run
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Split's sublevel in the splits tree
    /// </summary>
    public int Level { get; private set; }

    public string DisplayString { get; private set; }

    /// <summary>
    /// Is this spleat a leaf, meaning no subsplits
    /// </summary>
    public bool IsLeaf { get; private set; } = false;

    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection DeltaTimes = new();

    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection Times = new();

    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection SegmentTimes = new();

    [JsonInclude]
    public SplitInfo? PBSplit;
}

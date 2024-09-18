using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Splits;

public class SplitDisplayInfo
{
    public SplitDisplayInfo(string name, bool active, int level)
    {
        DisplayString = name;
        IsCurrent = active;
        Level = level;
    }

    public SplitDisplayInfo(Split s)
    {
        DisplayString = s.Name;
        IsCurrent = false;
        Level = 0;
    }

    public SplitInfo ToSplitInfo()
    {
        return new SplitInfo()
        {
            DeltaTime = DeltaTimes,
            Name = DisplayString,
            SegmentTime = SegmentTimes,
            TotalTime = Times
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

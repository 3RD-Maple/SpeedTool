using System.Text.Json.Serialization;
using SpeedTool.JSON;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Split
{
    public Split(string name)
    {
        Subsplits = [];
        SplitTimes = new();
        Name = name;
    }

    [JsonInclude]
    public string Name;

    [JsonInclude]
    public Split[] Subsplits;

    [JsonInclude]
    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection SplitTimes;

    public SplitDisplayInfo[] Flatten()
    {
        return Flatten(0);
    }

    public void AddSubsplit(Split split)
    {
        Subsplits = Subsplits.Append(split).ToArray();
    }

    public void InsertSplit(int idx, Split split)
    {
        Subsplits = Subsplits.InsertAt(idx, split);
    }

    private SplitDisplayInfo[] Flatten(int level)
    {
        List<SplitDisplayInfo> list = [new SplitDisplayInfo(Name, false, level)];

        foreach(var split in Subsplits)
        {
            list.AddRange(split.Flatten(level + 1));
        }

        return list.ToArray();
    }
}

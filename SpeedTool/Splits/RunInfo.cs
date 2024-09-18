using System.Text.Json.Serialization;
using SpeedTool.JSON;

namespace SpeedTool.Splits;

public class RunInfo
{
    public RunInfo() { }
    public RunInfo(string name, string cat, TimeCollection times, SplitInfo[] infos)
    {
        CategoryName = cat;
        GameName = name;
        TotalTimes = times;
        Splits = infos;
    }

    public string CategoryName { get; set; } = "";
    public string GameName { get; set; } = "";

    [JsonConverter(typeof(TimeCollectionConverter))]
    public TimeCollection TotalTimes { get; set; } = new();

    public SplitInfo[] Splits { get; set; } = [];
}

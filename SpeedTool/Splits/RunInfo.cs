using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class RunInfo
{
    public RunInfo(string name, string cat, TimeCollection times, SplitDisplayInfo[] infos)
    {
        CategoryName = cat;
        GameName = name;
        Times = times;
        Splits = infos;
    }

    public JsonObject ToJson()
    {
        JsonObject o = new();
        o["CategoryName"] = CategoryName;
        o["Times"] = Times.ToJson();
        o["GameName"] = GameName;

        JsonArray arr = new();
        for(int i = 0; i < Splits.Length; i++)
            arr.Add((JsonNode)Splits[i].ToJson());
        o["Splits"] = arr;
        return o;
    }

    public static RunInfo FromJson(JsonObject o)
    {
        RunInfo ret = new(o.EnforceGetString("GameName"), o.EnforceGetString("CategoryName"), new TimeCollection(o["Times"]!.AsObject()), SplitDisplayInfo.DeserializeJsonArray(o["Splits"]!.AsArray()));
        return ret;
    }

    public string CategoryName { get; private set; }
    public string GameName { get; private set; }
    public TimeCollection Times { get; private set; }

    public SplitDisplayInfo[] Splits { get; private set; }
}

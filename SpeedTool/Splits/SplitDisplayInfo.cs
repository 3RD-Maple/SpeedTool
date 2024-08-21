using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public struct SplitDisplayInfo
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

    public static SplitDisplayInfo FromJsonObject(JsonObject o)
    {
        SplitDisplayInfo ret = new SplitDisplayInfo(o.EnforceGetString("DisplayString"), false, (int)o["Level"]!);
        ret.Times = new TimeCollection(o["Times"]!.AsObject());
        ret.DeltaTimes = new TimeCollection(o["DeltaTimes"]!.AsObject());
        return ret;
    }

    public static JsonArray SerializeMany(SplitDisplayInfo[] splits)
    {
        JsonArray ret = new();
        for(int i = 0; i < splits.Length; i++)
            ret.Add((JsonNode)splits[i].ToJson());

        return ret;
    }

    public static SplitDisplayInfo[] DeserializeJsonArray(JsonArray array)
    {
        var count = array.Count;
        SplitDisplayInfo[] ret = new SplitDisplayInfo[count];
        for(int i = 0; i < count; i++)
            ret[i] = FromJsonObject(array[i]!.AsObject());
        return ret;
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

    public TimeCollection DeltaTimes = new();

    public TimeCollection Times = new();

    public JsonObject ToJson()
    {
        JsonObject o = new();
        o["Level"] = Level;
        o["DisplayString"] = DisplayString;
        o["DeltaTimes"] = DeltaTimes.ToJson();
        o["Times"] = Times.ToJson();

        return o;
    }
}

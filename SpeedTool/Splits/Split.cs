using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Split
{
    public Split()
    {
        Name = "";
        Subsplits = new Split[0];
        SplitTimes = new();
    }

    public Split(string name) : this()
    {
        Name = name;
    }

    public JsonObject ToJson()
    {
        JsonObject o = new();
        o["Name"] = Name;
        if(Subsplits != null && Subsplits.Length != 0)
        {
            o["Subsplits"] = SerializeSubsplits();
        }

        return o;
    }

    public static Split FromJson(JsonObject obj)
    {
        Split spl = new Split(obj.EnforceGetString("Name"));
        if(obj.ContainsKey("Subsplits"))
        {
            spl.LoadSubsplitsFromJson(obj["Subsplits"]!.AsArray());
        }
        return spl;
    }

    public string Name;

    public Split[] Subsplits;

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

    private void LoadSubsplitsFromJson(JsonArray array)
    {
        var len = array.Count;
        Subsplits = new Split[len];
        for(int i = 0; i < len; i++)
        {
            Subsplits[i] = Split.FromJson(array[i]!.AsObject());
        }
    }

    private JsonArray SerializeSubsplits()
    {
        JsonArray array = new();
        for(int i = 0; i < Subsplits.Length; i++)
        {
            array.Add((JsonNode)Subsplits[i].ToJson());
        }
        return array;
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

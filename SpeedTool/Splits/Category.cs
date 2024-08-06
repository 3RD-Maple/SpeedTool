using System.Text.Json;
using System.Text.Json.Nodes;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Category
{
    public Category(string name, Split[] splits)
    {
        Name = name;
        Splits = splits;
    }

    public JsonObject ToJson()
    {
        JsonObject o = new();
        o["Name"] = Name;
        o["RunsCount"] = RunsCount;
        o["splits"] = SerializeSplits();
        return o;
    }

    public static Category FromJson(JsonObject obj)
    {
        Category ret = new Category(obj.EnforceGetString("Name"), []);
        if(!obj.ContainsKey("Splits"))
            throw new FormatException();
        var splits = obj["Splits"]!.AsArray();
        var spl = new Split[splits.Count];
        for(int i = 0; i < spl.Length; i++)
        {
            spl[i] = Split.FromJson(splits[i]!.AsObject());
        }

        return ret;
    }

    public string Name { get; private set; }

    public Split[] Splits { get; private set; }

    public int RunsCount { get; private set; }

    private JsonArray SerializeSplits()
    {
        JsonArray arr = new();
        for(int i = 0; i < Splits.Length; i++)
        {
            arr.Add((JsonNode)Splits[i].ToJson());
        }

        return arr;
    }

    private static void EnforceField(JsonObject obj, string name)
    {
        if(obj[name] == null)
            throw new JsonException();
    }
}

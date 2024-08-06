using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SpeedTool.Util;

static class JSONHelper
{
    public static Vector4 Vector4FromJsonObject(JsonObject obj)
    {
        return new Vector4((float)obj["R"]!, (float)obj["G"]!, (float)obj["B"]!, (float)obj["A"]!);
    }

    public static string EnforceGetString(this JsonObject obj, string name)
    {
        if(!obj.ContainsKey(name))
            throw new FormatException();
        var ret = (string)obj[name]!;
        if(ret == null)
            throw new FormatException();
        
        return ret!;
    }

    public static JsonObject EnforceParseAsObject(string text)
    {
        var parsed = JsonNode.Parse(text);
        if(parsed == null)
            throw new JsonException();
        
        return parsed!.AsObject();
    }
}

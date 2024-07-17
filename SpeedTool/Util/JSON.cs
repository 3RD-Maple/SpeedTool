using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Util;

static class JSONHelper
{
    public static Vector4 Vector4FromJsonObject(JsonObject obj)
    {
        return new Vector4((float)obj["R"]!, (float)obj["G"]!, (float)obj["B"]!, (float)obj["A"]!);
    }
}

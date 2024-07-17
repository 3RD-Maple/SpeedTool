using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Util;

static class JSONHelper
{
    public static Vector4 Vector4FromJsonObject(JsonObject obj)
    {
        return new Vector4((float)obj["X"]!, (float)obj["Y"]!, (float)obj["Z"]!, (float)obj["W"]!);
    }
}

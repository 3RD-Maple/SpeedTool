using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Util;

public static class Vector4Extensions
{
    public static JsonObject ToJsonObject(this Vector4 vector)
    {
        var obj = new JsonObject();
        obj["R"] = vector.X;
        obj["G"] = vector.Y;
        obj["B"] = vector.Z;
        obj["A"] = vector.W;
        return obj;
    }
}

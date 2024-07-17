using System.Numerics;
using System.Text.Json.Nodes;

namespace SpeedTool.Util;

public static class Vector4Extensions
{
    public static JsonObject ToJsonObject(this Vector4 vector)
    {
        var obj = new JsonObject();
        obj["X"] = vector.X;
        obj["Y"] = vector.Y;
        obj["Z"] = vector.Z;
        obj["W"] = vector.W;
        return obj;
    }
}

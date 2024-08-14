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
    
    public static uint ToUint(this Vector4 vector)
    {
        byte r = (byte)(vector.X * 255.0f);
        byte g = (byte)(vector.Y * 255.0f);
        byte b = (byte)(vector.Z * 255.0f);
        byte a = (byte)(vector.W * 255.0f);
        
        uint packedColor = (uint)(a << 24 | b << 16 | g << 8 | r);
        
        return packedColor;
    }
}

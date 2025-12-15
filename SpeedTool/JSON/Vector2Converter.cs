using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpeedTool.JSON;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Invalid object");
        }

        float? x = default, y = default;

        for (int i = 0; i < 2; i++)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Invalid object");
            }

            var str = reader.GetString() ?? string.Empty;
            reader.Read();

            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException($"Invalid object");
            }

            if (!reader.TryGetDouble(out var tempF))
            {
                throw new JsonException($"Invalid value at {str}");
            }

            switch (str.ToLower())
            {
                case "x":
                    x = (float)tempF; break;
                case "y":
                    y = (float)tempF; break;
                default:
                    throw new JsonException("Invalid property name");
            }
        }

        if (x is null || y is null)
        {
            throw new JsonException("Missing value");
        }

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            reader.Read();
        }

        return new Vector2(x.Value, y.Value);
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("X",(double)value.X);
        writer.WriteNumber("Y",(double)value.Y);

        writer.WriteEndObject();
    }
}

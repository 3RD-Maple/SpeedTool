using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpeedTool.Global.Json
{
    /// <summary>
    /// Custom converter to support Vector4 structs
    /// </summary>
    internal class ColorVector4Converter : JsonConverter<Vector4>
    {
        ///<inheritdoc/>
        public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Invalid object");
            }

            float? x = default, y = default, z = default, w = default;

            for (int i = 0; i < 4; i++)
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
                    case "r":
                        x = (float)tempF; break;
                    case "g":
                        y = (float)tempF; break;
                    case "b":
                        z = (float)tempF; break;
                    case "a":
                        w = (float)tempF; break;
                    default:
                        throw new JsonException("Invalid property name");
                }
            }

            if (new[] {x, y, z, w }.Any(x=> x is null))
            {
                throw new JsonException("Missing value");
            }

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                reader.Read();
            }

            return new Vector4(x!.Value, y!.Value, z!.Value, w!.Value);
        }

        ///<inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("R",(double)value.X);
            writer.WriteNumber("G",(double)value.Y);
            writer.WriteNumber("B",(double)value.Z);
            writer.WriteNumber("A",(double)value.W);

            writer.WriteEndObject();
        }
    }
}

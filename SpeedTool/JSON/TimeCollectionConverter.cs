using System.Text.Json;
using System.Text.Json.Serialization;
using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.JSON;

public sealed class TimeCollectionConverter : JsonConverter<TimeCollection>
{
    public override TimeCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Invalid object");
        }

        TimeCollection tc = new();

        for(int i = 0; i < (int)TimingMethod.Last; i++)
        {
            reader.Read();
            if(reader.TokenType != JsonTokenType.Number)
                throw new JsonException("Expected number");

            tc[(TimingMethod)i] = new TimeSpan(reader.GetInt64());
        }

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            reader.Read();
        }

        return tc;
    }

    public override void Write(Utf8JsonWriter writer, TimeCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        for(int i = 0; i < (int)TimingMethod.Last; i++)
            writer.WriteNumberValue(value[(TimingMethod)i].Ticks);

        writer.WriteEndArray();
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;
using SharpHook.Native;
using SpeedTool.Platform;

namespace SpeedTool.JSON;

public sealed class HotkeyConverter : JsonConverter<Hotkey>
{
    public override Hotkey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Invalid object");
        }

        bool alt = false, shift = false, ctrl = false;
        int keyCode = 0;

        for (int i = 0; i < 4; i++)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Invalid object");
            }

            var str = reader.GetString() ?? string.Empty;
            reader.Read();

            if (reader.TokenType == JsonTokenType.Number && str == "KeyCode")
            {
                if(!reader.TryGetInt32(out keyCode))
                    throw new JsonException($"Invalid value for {str}");
                continue;
            }

            if(reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                var val = reader.TokenType == JsonTokenType.True ? true : false;
                switch(str)
                {
                case "CheckAlt":
                    alt = val;
                    break;
                case "CheckShift":
                    shift = val;
                    break;
                case "CheckCtrl":
                    ctrl = val;
                    break;
                }
                continue;
            }

            throw new JsonException($"Invalid value at {str}");
        }

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            reader.Read();
        }

        return new Hotkey()
        {
            Alt = alt,
            Ctrl = ctrl,
            Shift = shift,
            Key = (KeyCode)keyCode
        };
    }

    public override void Write(Utf8JsonWriter writer, Hotkey value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteBoolean("CheckAlt", value.Alt);
        writer.WriteBoolean("CheckShift", value.Shift);
        writer.WriteBoolean("CheckCtrl", value.Ctrl);
        writer.WriteNumber("KeyCode", (int)value.Key);

        writer.WriteEndObject();
    }
}

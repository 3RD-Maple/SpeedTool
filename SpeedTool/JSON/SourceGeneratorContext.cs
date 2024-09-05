using System.Text.Json.Serialization;
using SpeedTool.Global.Definitions;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ClassicUISettings))]
[JsonSerializable(typeof(ColorSettings))]
[JsonSerializable(typeof(GeneralConfiguration))]
[JsonSerializable(typeof(HotkeySettings))]
[JsonSerializable(typeof(SpeedToolUISettings))]
[JsonSerializable(typeof(SpeedToolUITheme))]
internal partial class SourceGeneratorContext : JsonSerializerContext {}

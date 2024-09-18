using System.Text.Json.Serialization;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ClassicUISettings))]
[JsonSerializable(typeof(ColorSettings))]
[JsonSerializable(typeof(GeneralConfiguration))]
[JsonSerializable(typeof(HotkeySettings))]
[JsonSerializable(typeof(SpeedToolUISettings))]
[JsonSerializable(typeof(SpeedToolUITheme))]
[JsonSerializable(typeof(SplitInfo))]
[JsonSerializable(typeof(SplitDisplayInfo))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(RunInfo))]
internal partial class SourceGeneratorContext : JsonSerializerContext {}

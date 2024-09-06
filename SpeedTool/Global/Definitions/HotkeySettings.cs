using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SpeedTool.JSON;
using SpeedTool.Platform;

namespace SpeedTool.Global.Definitions;

public sealed class HotkeySettings : IConfigurationSection
{
    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey SplitHotkey;

    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey NextSplitHotkey;

    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey PreviousSplitHotkey;
    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey PauseHotkey;
    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey ResetHotkey;

    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey NextCategoryHotkey;
    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey PreviousCategoryHotkey;

    [JsonInclude]
    [JsonConverter(typeof(HotkeyConverter))]
    public Hotkey ToggleHotkeysHotkey;

    [JsonInclude]
    public bool HotkeysEnabled = true;
}

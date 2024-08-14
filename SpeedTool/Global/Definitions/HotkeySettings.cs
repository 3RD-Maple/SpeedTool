using System.Text.Json.Nodes;
using SpeedTool.Platform;

namespace SpeedTool.Global.Definitions;

public sealed class HotkeySettings : IConfigurationSection
{
    public Hotkey SplitHotkey;
    public Hotkey NextSplitHotkey;
    public Hotkey PreviousSplitHotkey;

    public Hotkey PauseHotkey;
    public Hotkey ResetHotkey;

    public Hotkey NextCategoryHotkey;
    public Hotkey PreviousCategoryHotkey;

    public Hotkey ToggleHotkeysHotkey;

    public bool HotkeysEnabled = true;

    public void FromJSONObject(JsonObject node)
    {
        SplitHotkey = Hotkey.FromJSONObject(node["SplitHotkey"]!.AsObject());
        NextSplitHotkey = Hotkey.FromJSONObject(node["NextSplitHotkey"]!.AsObject());
        PreviousSplitHotkey = Hotkey.FromJSONObject(node["PreviousSplitHotkey"]!.AsObject());

        PauseHotkey = Hotkey.FromJSONObject(node["PauseHotkey"]!.AsObject());
        ResetHotkey = Hotkey.FromJSONObject(node["ResetHotkey"]!.AsObject());

        NextCategoryHotkey = Hotkey.FromJSONObject(node["NextCategoryHotkey"]!.AsObject());
        PreviousCategoryHotkey = Hotkey.FromJSONObject(node["PreviousCategoryHotkey"]!.AsObject());
        ToggleHotkeysHotkey = Hotkey.FromJSONObject(node["ToggleHotkeysHotkey"]!.AsObject());

        HotkeysEnabled = (bool)node["HotkeysEnabled"]!;
    }

    public JsonObject ToJSONObject()
    {
        JsonObject n = new();
        n["SplitHotkey"] = Hotkey.ToJSONObject(SplitHotkey);
        n["NextSplitHotkey"] = Hotkey.ToJSONObject(NextSplitHotkey);
        n["PreviousSplitHotkey"] = Hotkey.ToJSONObject(PreviousSplitHotkey);

        n["PauseHotkey"] = Hotkey.ToJSONObject(PauseHotkey);
        n["ResetHotkey"] = Hotkey.ToJSONObject(ResetHotkey);

        n["NextCategoryHotkey"] = Hotkey.ToJSONObject(NextCategoryHotkey);
        n["PreviousCategoryHotkey"] = Hotkey.ToJSONObject(PreviousCategoryHotkey);
        n["ToggleHotkeysHotkey"] = Hotkey.ToJSONObject(ToggleHotkeysHotkey);

        n["HotkeysEnabled"] = HotkeysEnabled;

        return n;
    }
}

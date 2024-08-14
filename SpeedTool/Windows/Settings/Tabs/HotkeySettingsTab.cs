using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class HotkeySettingsTab : TabBase
{
    private HotkeySettings settings;

    public HotkeySettingsTab(string tabName) : base(tabName)
    {
        settings = Configuration.GetSection<HotkeySettings>()!;
    }

    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(settings);
    }

    protected override void DoTabInternal()
    {
        ImGui.Checkbox("Hotkeys enabled", ref settings.HotkeysEnabled);

        ImGui.Text("Timer controls:");
        ImGuiExtensions.SpeedToolHotkey("Split", ref settings.SplitHotkey);
        ImGuiExtensions.SpeedToolHotkey("Pause", ref settings.PauseHotkey);
        ImGuiExtensions.SpeedToolHotkey("Reset", ref settings.ResetHotkey);
        ImGuiExtensions.SpeedToolHotkey("Next Split", ref settings.NextSplitHotkey);
        ImGuiExtensions.SpeedToolHotkey("Previous Split", ref settings.PreviousSplitHotkey);
        ImGui.Separator();

        ImGui.Text("Category controls:");
        ImGuiExtensions.SpeedToolHotkey("Next Category", ref settings.NextCategoryHotkey);
        ImGuiExtensions.SpeedToolHotkey("Previous Category", ref settings.PreviousCategoryHotkey);
        ImGui.Separator();

        ImGui.Text("Other:");
        ImGuiExtensions.SpeedToolHotkey("Toggle Hotkeys", ref settings.ToggleHotkeysHotkey);
    }
}

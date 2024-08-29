using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class HotkeySettingsTab : TabBase
{
    private HotkeySettings Config;

    public HotkeySettingsTab(string tabName) : base(tabName)
    {
        Config = Configuration.GetSection<HotkeySettings>()!;
    }

    protected override void ApplyTabSettings()
    {
       Configuration.SetSection(Config);
    }
    
    protected override void OnConfigChanges(object? sender, IConfigurationSection section)
    {
        if (!(section is HotkeySettings))
            return;
        
        // event handling 
    }

    protected override void DoTabInternal()
    {
        ImGui.Checkbox("Hotkeys enabled", ref Config.HotkeysEnabled);

        ImGui.Text("Timer controls:");
        ImGuiExtensions.SpeedToolHotkey("Split", ref Config.SplitHotkey);
        ImGuiExtensions.SpeedToolHotkey("Pause", ref Config.PauseHotkey);
        ImGuiExtensions.SpeedToolHotkey("Reset", ref Config.ResetHotkey);
        ImGuiExtensions.SpeedToolHotkey("Next Split", ref Config.NextSplitHotkey);
        ImGuiExtensions.SpeedToolHotkey("Previous Split", ref Config.PreviousSplitHotkey);
        ImGui.Separator();

        ImGui.Text("Category controls:");
        ImGuiExtensions.SpeedToolHotkey("Next Category", ref Config.NextCategoryHotkey);
        ImGuiExtensions.SpeedToolHotkey("Previous Category", ref Config.PreviousCategoryHotkey);
        ImGui.Separator();

        ImGui.Text("Other:");
        ImGuiExtensions.SpeedToolHotkey("Toggle Hotkeys", ref Config.ToggleHotkeysHotkey);
    }
}

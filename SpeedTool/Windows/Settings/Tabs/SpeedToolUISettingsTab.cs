using System.Numerics;
using SpeedTool.Platform;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class SpeedToolUISettingsTab : TabBase
{
    private Vector4 secondsClockTimerColor = new Vector4(1);
    private Vector4 minutesClockTimerColor = new Vector4(1);
    private Vector4 hoursClockTimerColor = new Vector4(1);
    
    public SpeedToolUISettingsTab(string tabName) : base(tabName) {}
    
    protected override void DoTabInternal()
    {
        ImGuiExtensions.SpeedToolColorPicker("Seconds color", ref secondsClockTimerColor);
        ImGuiExtensions.SpeedToolColorPicker("Minutes color", ref minutesClockTimerColor);
        ImGuiExtensions.SpeedToolColorPicker("Hours color", ref hoursClockTimerColor);

        // Usage reference
        ImGuiExtensions.SpeedToolHotkey("Hotkey Test", ref hk);
        ImGuiExtensions.SpeedToolHotkey("Hotkey 2", ref hk2);
    }

    private Hotkey hk;
    private Hotkey hk2;
}
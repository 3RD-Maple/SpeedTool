using System.Numerics;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Platform;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class SpeedToolUISettingsTab : TabBase
{
    private SpeedToolUISettings Config { get; } =
        Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();

    private Vector4 secondsClockTimerColor;
    private Vector4 minutesClockTimerColor;
    private Vector4 hoursClockTimerColor;

    private Hotkey startHotkey;
    private Hotkey pauseHotkey;
    private Hotkey resetHotkey;

    public SpeedToolUISettingsTab(string tabName) : base(tabName)
    {
        secondsClockTimerColor = Config.SecondsClockTimerColor;
        minutesClockTimerColor = Config.MinutesClockTimerColor;
        hoursClockTimerColor = Config.HoursClockTimerColor;
        
        startHotkey = Config.StartHotkey;
        pauseHotkey = Config.PauseHotkey;
        resetHotkey = Config.ResetHotkey;
    }

    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(Config);
    }

    protected override void DoTabInternal()
    {
        SettingsWindow.SpeedToolThemeWindow(Config);

        ImGuiExtensions.SpeedToolHotkey("Start", ref Config.StartHotkey);
        ImGuiExtensions.SpeedToolHotkey("Pause", ref Config.PauseHotkey);
        ImGuiExtensions.SpeedToolHotkey("Reset", ref Config.ResetHotkey);
    }
}

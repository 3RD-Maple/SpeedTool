using System.Numerics;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class SpeedToolUISettingsTab : TabBase
{
    private SpeedToolUISettings Config { get; } =
        Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();

    private Vector4 secondsClockTimerColor;
    private Vector4 minutesClockTimerColor;
    private Vector4 hoursClockTimerColor;

    public SpeedToolUISettingsTab(string tabName) : base(tabName)
    {
        secondsClockTimerColor = Config.SecondsClockTimerColor;
        minutesClockTimerColor = Config.MinutesClockTimerColor;
        hoursClockTimerColor = Config.HoursClockTimerColor;
    }

    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(Config);
    }

    protected override void DoTabInternal()
    {
        SettingsWindow.SpeedToolThemeWindow(Config);
    }
}

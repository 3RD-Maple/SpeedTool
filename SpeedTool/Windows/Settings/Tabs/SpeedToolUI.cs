using System.Numerics;
using ImGuiNET;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class SpeedToolUI : TabBase
{
    private Vector4 secondsClockTimerColor = new Vector4(1);
    private Vector4 minutesClockTimerColor = new Vector4(1);
    private Vector4 hoursClockTimerColor = new Vector4(1);
    
    public override void DoTab()
    {
        if(ImGui.BeginTabItem("SpeedToolUI"))
        {
            ImGuiExtensions.SpeedToolColorPicker("Seconds color", ref secondsClockTimerColor);
            ImGuiExtensions.SpeedToolColorPicker("Minutes color", ref minutesClockTimerColor);
            ImGuiExtensions.SpeedToolColorPicker("Hours color", ref hoursClockTimerColor);
            
            ImGui.EndTabItem();
        }
    }
}
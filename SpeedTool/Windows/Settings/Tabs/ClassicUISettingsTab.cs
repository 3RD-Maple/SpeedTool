using System.Numerics;
using ImGuiNET;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ClassicUISettingsTab : TabBase
{
    private Vector4 activeSplitColor = new Vector4(1);
    private int ShownSplitsCount = 5;
    
    public override void DoTab()
    {
        if (ImGui.BeginTabItem("ClassicUI"))
        {
            ImGuiExtensions.SpeedToolColorPicker("Active split", ref activeSplitColor);
            ImGui.Text("Shown splits");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(75f);
            ImGui.InputInt("", ref ShownSplitsCount);
        }
    }
}
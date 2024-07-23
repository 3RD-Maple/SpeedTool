using System.Numerics;
using ImGuiNET;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ClassicUISettingsTab : TabBase
{
    
    private Vector4 activeSplitColor = new Vector4(1);
    private int shownSplitsCount = 5;
    
    public ClassicUISettingsTab(string tabName) : base(tabName){}
    protected override void DoTabInternal()
    {
        ImGuiExtensions.SpeedToolColorPicker("Active split", ref activeSplitColor);
        ImGui.Text("Shown splits");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75f);
        ImGui.InputInt("", ref shownSplitsCount);
    }
}
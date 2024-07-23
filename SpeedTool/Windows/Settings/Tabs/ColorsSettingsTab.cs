using System.Numerics;
using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ColorsSettingsTab : TabBase
{
    private Vector4 textColor = new Vector4(1); //to be removed. used as a temp variable
    private Vector4 aheadColor = new Vector4(1);
    private Vector4 behindColor = new Vector4(1);
    private Vector4 PBColor = new Vector4(1);

    public ColorsSettingsTab(string tabName) : base(tabName) {}

protected override void DoTabInternal()
    {
        ImGuiExtensions.SpeedToolColorPicker("Text color", ref textColor);
        ImGuiExtensions.SpeedToolColorPicker("Ahead time color", ref aheadColor);
        ImGuiExtensions.SpeedToolColorPicker("Behind time color", ref behindColor);
        ImGuiExtensions.SpeedToolColorPicker("PB color", ref PBColor);
    }
}
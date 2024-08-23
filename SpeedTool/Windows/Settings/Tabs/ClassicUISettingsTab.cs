using System.Numerics;
using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ClassicUISettingsTab : TabBase
{
    private ClassicUISettings Config { get; } =
        Configuration.GetSection<ClassicUISettings>() ?? throw new Exception();

    private Vector4 activeSplitColor;
    private int shownSplitsCount;

    public ClassicUISettingsTab(string tabName) : base(tabName)
    {
        activeSplitColor = Config.ActiveSplitColor;
        shownSplitsCount = Config.ShownSplitsCount;
    }
    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(Config);
    }

    protected override void DoTabInternal()
    {
        ImGuiExtensions.SpeedToolColorPicker("Active split", ref Config.ActiveSplitColor);
        ImGui.InputInt("Shown Splits", ref Config.ShownSplitsCount, 1, 1);
    }
}
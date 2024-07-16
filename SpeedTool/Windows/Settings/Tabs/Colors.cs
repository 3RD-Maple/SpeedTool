using System.Numerics;
using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class Colors : TabBase
{
    private Vector4 TextColor = new Vector4(1); //to be removed. used as a temp variable
    private Vector4 AheadColor = new Vector4(1);
    private Vector4 BehindColor = new Vector4(1);
    private  Vector4 PBColor = new Vector4(1);
    
    public override void DoTab()
    {
        if (ImGui.BeginTabItem("Colors"))
        {
            ImGuiExtensions.SpeedToolColorPicker("Text color", ref TextColor);
            ImGuiExtensions.SpeedToolColorPicker("Ahead time color", ref AheadColor);
            ImGuiExtensions.SpeedToolColorPicker("Behind time color", ref BehindColor);
            ImGuiExtensions.SpeedToolColorPicker("PB color", ref PBColor);

            if (ImGui.Button("Save changes"))
            {
                var genConf = Configuration.GetSection<GeneralConfiguration>() ?? throw new Exception();
                genConf.TextColor = TextColor;
                var write = Configuration.SetSection<GeneralConfiguration>(genConf);  
            }

            ImGui.EndTabItem();
        }
    }
}
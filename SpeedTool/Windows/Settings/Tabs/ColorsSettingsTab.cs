using System.Numerics;

using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ColorsSettingsTab : TabBase
{
   private ColorSettings Config { get; } =
       Configuration.GetSection<ColorSettings>() ?? throw new Exception();

   private Vector4 textColor;
   private Vector4 aheadColor;
   private Vector4 behindColor;
   private Vector4 PBColor;

   public ColorsSettingsTab(string tabName) : base(tabName)
   {
       textColor = Config.TextColor;
       aheadColor = Config.AheadColor;
       behindColor = Config.BehindColor;
       PBColor = Config.PBColor;
   }
    
    
    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(Config);
    }

    protected override void DoTabInternal()
    {
        ImGuiExtensions.SpeedToolColorPicker("Text color", ref Config.TextColor);
        ImGuiExtensions.SpeedToolColorPicker("Ahead time color", ref Config.AheadColor);
        ImGuiExtensions.SpeedToolColorPicker("Behind time color", ref Config.BehindColor);
        ImGuiExtensions.SpeedToolColorPicker("PB color", ref Config.PBColor);
    }
}
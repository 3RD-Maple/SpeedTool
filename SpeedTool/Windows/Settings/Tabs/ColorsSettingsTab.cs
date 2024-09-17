using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class ColorsSettingsTab : TabBase
{
   private ColorSettings Config { get; } =
       Configuration.GetSection<ColorSettings>() ?? throw new Exception();

   public ColorsSettingsTab(string tabName) : base(tabName) { }

   protected override void OnConfigChanges(object? sender, IConfigurationSection section)
   {
       if (!(section is ColorSettings))
           return;

        // TODO: ??? What's going on here?
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
        ImGui.Separator();
        ImGuiExtensions.SpeedToolColorPicker("Timer Background Color", ref Config.TimerBackground);
    }
}
using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Windows.Settings;
using SpeedTool.Windows.Settings.Tabs;
using Window = SpeedTool.Platform.Window;

namespace SpeedTool.Windows;

public sealed class SettingsWindow() : Window(options, new Vector2D<int>(500, 550))
{
    private readonly TabBase[] settingsWindowTabs = new TabBase[]
            { new ColorsSettingsTab("Colors"), new ClassicUISettingsTab("ClassicUI"), new SpeedToolUISettingsTab("SpeedToolUI") }
            .ToArray();

    private static WindowOptions options
    {
        get
        {
            var opts = WindowOptions.Default;
            opts.Samples = 8;
            opts.WindowBorder = WindowBorder.Fixed;
            return opts;
        }
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("SettingsWindow",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoMove);

        if (ImGui.BeginTabBar("Settings window"))
        {
            foreach (var tab in settingsWindowTabs)
                tab.DoTab();
        }
        
        
        if (ImGui.Button("Apply changes"))
        {
            var genConf = Configuration.GetSection<GeneralConfiguration>() ?? throw new Exception();
            //
            var write = Configuration.SetSection<GeneralConfiguration>(genConf);  
        }


        ImGui.End();
    }
}
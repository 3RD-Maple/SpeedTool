using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Windows.Settings;
using SpeedTool.Windows.Settings.Tabs;
using Window = SpeedTool.Platform.Window;

namespace SpeedTool.Windows;

public sealed class SettingsWindow : Window
{
    private Platform.Platform platform = Platform.Platform.SharedPlatform;

    private List<TabBase> SettingsWindowTabs;

    public SettingsWindow() : base(options, new Vector2D<int>(500, 550))
    {
        SettingsWindowTabs = new TabBase[]
                { new Colors(), new ClassicUI(), new SpeedToolUI() }
            .ToList();
    }

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

        if (ImGui.BeginTabBar("SettingsTabs"))
        {
            foreach (var tab in SettingsWindowTabs) tab.DoTab();
            ImGui.EndTabBar();
        }

        ImGui.End();
    }
}
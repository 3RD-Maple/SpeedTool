using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;
using SpeedTool.Windows.Settings;
using SpeedTool.Windows.Settings.Tabs;
using Window = SpeedTool.Platform.Window;

namespace SpeedTool.Windows;

public sealed class SettingsWindow() : Window(options, new Vector2D<int>(500, 550))
{
    private readonly TabBase[] settingsWindowTabs =
    {
        new ColorsSettingsTab("Colors"), new ClassicUISettingsTab("ClassicUI"),
        new SpeedToolUISettingsTab("SpeedToolUI")
    };

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
            foreach (var tab in settingsWindowTabs)
            {
                tab.ApplySettings();
            }
        }

        ImGui.End();
    }
    
    public static void SpeedToolThemeWindow(SpeedToolUISettings config)
    {
        string[] themes = { "Classic", "Viola", "Melon", "Custom" };
        
        if (ImGui.BeginCombo("Theme", config.Theme))
        {
            foreach (var theme in themes)
            {
                bool isSelected = config.Theme == theme;


                if (ImGui.Selectable(theme, isSelected))
                {
                    config.Theme = theme;
                    
                    switch (config.Theme)
                    {
                        case "Classic":
                            config.SecondsClockTimerColor = new Vector4(1, 0, 0, 1f);
                            config.MinutesClockTimerColor = new Vector4(0, 1, 0, 1f);
                            config.HoursClockTimerColor = new Vector4(0, 0, 1, 1f);
                            break;
                        
                        case "Viola":
                            config.SecondsClockTimerColor = new Vector4(0.5f, 0f, 1, 1f);
                            config.MinutesClockTimerColor = new Vector4(0.924f, 0.962f, 0.475f, 1f);
                            config.HoursClockTimerColor = new Vector4(0.470f, 0, 1, 1f);
                            break;

                        case "Melon":
                            config.SecondsClockTimerColor = new Vector4(0.9f, 1, 0, 1f);
                            config.MinutesClockTimerColor = new Vector4(0.1f, 0.4f, 0, 1f);
                            config.HoursClockTimerColor = new Vector4(0.8f, 0.2f, 0.4f, 1f);
                            break;
                    }

                    Configuration.SetSection(config);
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        if (config.Theme == "Custom")
        {
            ImGuiExtensions.SpeedToolColorPicker("Seconds color", ref config.SecondsClockTimerColor);
            ImGuiExtensions.SpeedToolColorPicker("Minutes color", ref config.MinutesClockTimerColor);
            ImGuiExtensions.SpeedToolColorPicker("Hours color", ref config.HoursClockTimerColor);
        }
    }
}

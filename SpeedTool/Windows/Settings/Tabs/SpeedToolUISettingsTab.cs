using System.Numerics;
using System.Text.Json.Nodes;
using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.Settings.Tabs;

public sealed class SpeedToolUISettingsTab : TabBase
{
    private SpeedToolUISettings Config { get; } =
        Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();

    private Vector4 secondsClockTimerColor;
    private Vector4 minutesClockTimerColor;
    private Vector4 hoursClockTimerColor;
    private Dictionary<string, SpeedToolUITheme> themes;
    

    public SpeedToolUISettingsTab(string tabName) : base(tabName)
    {
        secondsClockTimerColor = Config.SecondsClockTimerColor;
        minutesClockTimerColor = Config.MinutesClockTimerColor;
        hoursClockTimerColor = Config.HoursClockTimerColor;
        
        using (var stream = typeof(Program).Assembly.GetManifestResourceStream(RESOURCE_NAME)!)
        {
            var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            var jsonObject = JsonNode.Parse(content)!.AsObject();

            themes = jsonObject.ToDictionary(
                kvp => kvp.Key,
                kvp => new SpeedToolUITheme(kvp.Value!.AsObject())
            );
        }
    }

    protected override void ApplyTabSettings()
    {
        Configuration.SetSection(Config);
    }

    protected override void DoTabInternal()
    {
        SpeedToolThemeWindow(Config);
    }

    public void SpeedToolThemeWindow(SpeedToolUISettings config)
    {
       

        if (ImGui.BeginCombo("Theme", config.Theme))
        {
            foreach (var theme in themes)
            {
                bool isSelected = config.Theme == theme.Key;

                if (ImGui.Selectable(theme.Key, isSelected))
                {
                    config.Theme = theme.Key;
                    
                    if (themes.TryGetValue(config.Theme, out var selectedTheme))
                    {
                        config.SecondsClockTimerColor = selectedTheme.SecondsClockTimerColor;
                        config.MinutesClockTimerColor = selectedTheme.MinutesClockTimerColor;
                        config.HoursClockTimerColor = selectedTheme.HoursClockTimerColor;
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

    private const string RESOURCE_NAME = "SpeedTool.Resources.themes.json";
}

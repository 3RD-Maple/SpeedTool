using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;
using SpeedTool.Windows.Drawables;

namespace SpeedTool.Windows.TimerUI;

internal class SpeedToolTimerUI : TimerUIBase
{
    private readonly TimerDrawable drw;
    private readonly GL gl;

    private string stringShortened = "some split";

    public SpeedToolTimerUI(GL gl)
    {
        this.gl = gl;
        drw = new TimerDrawable(gl);
    }

    private ColorSettings colorsConfig { get; set; } =
        Configuration.GetSection<ColorSettings>() ?? throw new Exception();

    private SpeedToolUISettings speedToolConfig { get; set; } =
        Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();

    private string currentSplit { get; set; } = "";

    public override void Draw(double dt, ISplitsSource splits, ITimerSource timer)
    {
        speedToolConfig = Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();
        drw.SecondsColor = speedToolConfig.SecondsClockTimerColor;
        drw.MinutesColor = speedToolConfig.MinutesClockTimerColor;
        drw.HoursColor = speedToolConfig.HoursClockTimerColor;
        gl.Viewport(0, 0, 500, 500);
        drw.Draw(timer, speedToolConfig);
    }
    
    public override void ReloadConfig(object? sender, IConfigurationSection? section)
    {
        if((section as ColorSettings) != null)
        colorsConfig = (section as ColorSettings)!;

        if((section as SpeedToolUISettings) != null)
            speedToolConfig = (section as SpeedToolUISettings)!;
    }

    public override void DoUI(ISplitsSource splits, ITimerSource timer)
    {
        var width = ImGui.GetWindowWidth() * 0.6f; // working area is about 2/3 of the entire window size

        if (splits.CurrentSplit.DisplayString != currentSplit) //if curSplit != CurrentSplit
        {
            currentSplit = splits.CurrentSplit.DisplayString;
            stringShortened = ImGuiExtensions.ShortenStringForWidth((int)width, currentSplit);
        }

        colorsConfig = Configuration.GetSection<ColorSettings>() ?? throw new Exception();
        var style = ImGui.GetStyle();
        style.FramePadding = new Vector2(0, 0);
        style.ItemSpacing = new Vector2(0, 0);
        style.WindowPadding = new Vector2(0, 0);
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.00f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.Text, colorsConfig.TextColor);
        DoStartButton(timer);
        ImGui.SameLine();
        DoPauseButton(timer);
        ImGui.PopStyleColor(5);
        if (Platform.Platform.SharedPlatform.Game == null)
            TextCentered("No game");
        else
            TextCentered(stringShortened);

        if(splits.PreviousSplit != null)
            DrawDtText(splits.PreviousSplit!.Value);
        DrawTimeText(timer);
    }

    private void SetTextCenter(string text)
    {
        var ts = new Vector2(100, 100);
        var ws = ImGui.GetWindowSize();

        if (!string.IsNullOrEmpty(text)) ts = ImGui.CalcTextSize(text);

        ImGui.SetCursorPos(new Vector2((ws.X - ts.X) / 2, (ws.Y - ts.Y) / 2));
    }

    private void TextCentered(string text)
    {
        SetTextCenter(text);
        ImGui.Text(text);
    }

    private void DrawDtText(SplitDisplayInfo split)
    {
        var ahead = split.DeltaTimes[DisplayTimingMethod].Ticks < 0;
        var color = ahead ? colorsConfig.AheadColor : colorsConfig.BehindColor;
        var text = split.DeltaTimes[DisplayTimingMethod].ToSpeedtoolDTString();
        var sz = ImGui.CalcTextSize(text).X;
        ImGui.SetCursorPos(new Vector2(250 - sz / 2, 300));
        ImGui.TextColored(color, text);
    }

    private void DrawTimeText(ITimerSource timer)
    {
        var text = timer.CurrentTime.ToSpeedToolTimerString();
        var sz = ImGui.CalcTextSize(text);
        ImGui.SetCursorPos(new Vector2(250 - sz.X / 2, 350));
        ImGui.TextColored(colorsConfig.TextColor, text);
    }

    private void DoStartButton(ITimerSource timer)
    {
        var sz = new Vector2(250, 50);
        switch (timer.CurrentState)
        {
            case TimerState.NoState:
                if (ImGui.Button("Start", sz))
                    timer.Start();
                break;
            case TimerState.Running:
                if (ImGui.Button("Split", sz))
                    timer.Stop();
                break;
            default:
                if (ImGui.Button("Reset", sz))
                    timer.Reset();
                break;
        }
    }

    private void DoPauseButton(ITimerSource timer)
    {
        var sz = new Vector2(250, 50);
        var text = timer.CurrentState == TimerState.Paused ? "Unpause" : "Pause";
        if (ImGui.Button(text, sz))
            Platform.Platform.SharedPlatform.Pause();
    }
}
using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Windows.Drawables;

namespace SpeedTool.Windows.TimerUI;

class SpeedToolTimerUI : TimerUIBase
{
    private readonly GL gl;
    private readonly TimerDrawable drw;
    private ColorSettings colorsConfig { get; set; } = 
        Configuration.GetSection<ColorSettings>() ?? throw new Exception();
    
    private SpeedToolUISettings speedToolConfig { get; set; } = 
        Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();

    private string currentSplit { get; set; } = "";

    public SpeedToolTimerUI(GL gl)
    {
        this.gl = gl;
        drw = new TimerDrawable(gl);
    }
    public override void Draw(double dt, ISplitsSource splits, ITimerSource timer)
    {
        speedToolConfig = Configuration.GetSection<SpeedToolUISettings>() ?? throw new Exception();
        drw.SecondsColor = speedToolConfig.SecondsClockTimerColor;
        drw.MinutesColor = speedToolConfig.MinutesClockTimerColor;
        drw.HoursColor = speedToolConfig.HoursClockTimerColor;
        gl.Viewport(0, 0, 500, 500);
        drw.Draw(timer, speedToolConfig);
    }

    public override void DoUI(ISplitsSource splits, ITimerSource timer)
    {
        if (splits.CurrentSplit.DisplayString != currentSplit) //if curSplit != CurrentSplit
        {
            currentSplit = splits.CurrentSplit.DisplayString;
        }
        else
        {
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
            TextCentered(currentSplit);
            DrawTimeText(timer);
        }
        
    }
    
    private void SetTextCenter(string text)
    {
        var ts = new Vector2(100, 100);
        var ws = ImGui.GetWindowSize();

        if (!String.IsNullOrEmpty(text))
        {
            ts = ImGui.CalcTextSize(text);
        }
        
        ImGui.SetCursorPos(new Vector2((ws.X - ts.X) / 2, (ws.Y - ts.Y) / 2));
    }

    private void TextCentered(string text)
    {
        var threeDotsSize = ImGui.CalcTextSize("...").X;
        var width = ImGui.GetWindowWidth() * 0.6f; // working area is about 2/3 of the entire window size

        if (String.IsNullOrEmpty(text))
        {
            SetTextCenter(text);
            ImGui.Text("");
            return;
        }

        if (ImGui.CalcTextSize(text).X <= width)
        {
            ImGui.Text(text);
            return;
        }

        var symbols = text.Length;

        while ((threeDotsSize + ImGui.CalcTextSize(text.AsSpan(0, symbols)).X) > width)
        {
            symbols--;
        }

        var textResult = text.Substring(0, symbols) + "...";
        
        SetTextCenter(textResult);
        ImGui.Text(textResult);
    }
    
    private void DrawTimeText(ITimerSource timer)
    {
        var text = timer.CurrentTime.ToSpeedToolTimerString();
        var sz = ImGui.CalcTextSize(text);
        ImGui.SetCursorPos(new Vector2(250 - sz.X / 2, 300));
        ImGui.TextColored(colorsConfig.TextColor, text);
    }

    private void DoStartButton(ITimerSource timer)
    {
        Vector2 sz = new Vector2(250, 50);
        switch(timer.CurrentState)
        {
            case TimerState.NoState:
                if(ImGui.Button("Start", sz))
                    timer.Start();
                break;
            case TimerState.Running:
                if(ImGui.Button("Split", sz))
                    timer.Stop();
                break;
            default:
                if(ImGui.Button("Reset", sz))
                    timer.Reset();
                break;
        }
    }

    private void DoPauseButton(ITimerSource timer)
    {
        Vector2 sz = new Vector2(250, 50);
        string text = timer.CurrentState == TimerState.Paused ? "Unpause" : "Pause";
        if(ImGui.Button(text, sz))
            timer.Pause();
    }
}

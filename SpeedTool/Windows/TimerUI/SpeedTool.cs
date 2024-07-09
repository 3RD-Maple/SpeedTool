using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Windows.Drawables;

namespace SpeedTool.Windows.TimerUI;

class SpeedToolTimerUI : TimerUI
{
    public SpeedToolTimerUI(GL gl)
    {
        this.gl = gl;
        drw = new TimerDrawable(gl);
    }
    public override void Draw(double dt, ITimerSource timer)
    {
        gl.Viewport(0, 0, 500, 500);
        drw.Draw(timer);
    }

    public override void DoUI(ITimerSource timer)
    {
        var style = ImGui.GetStyle();
        style.FramePadding = new Vector2(0, 0);
        style.ItemSpacing = new Vector2(0, 0);
        style.WindowPadding = new Vector2(0, 0);
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.00f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
        DoStartButton(timer);
        ImGui.SameLine();
        DoPauseButton(timer);
        ImGui.PopStyleColor(4);

        DrawTimeText(timer);
    }

    private void DrawTimeText(ITimerSource timer)
    {
        var text = timer.CurrentTime.ToSpeedToolTimerString();
        var sz = ImGui.CalcTextSize(text);
        ImGui.SetCursorPos(new Vector2(250 - sz.X / 2, 300));
        ImGui.Text(text);
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

    private GL gl;
    private TimerDrawable drw;
}

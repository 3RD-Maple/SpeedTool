using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;
using Window = SpeedTool.Platform.Window;

namespace SpeedTool.Windows;

class TimeEditorWindow : Window
{
    public TimeEditorWindow(Split[] splits) : base(options, new Vector2D<int>(500, 500))
    {
        this.splits = splits;
    }

    private Split[] splits;

    private static WindowOptions options
    {
        get
        {
            var opts = WindowOptions.Default;
            opts.Samples = 8;
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

        ImGui.PushFont(GetFont("UI"));

        for(int i = 0; i < splits.Length; i++)
        {
            DrawSplit(splits[i]);
        }

        ImGui.PopFont();

        ImGui.End();
    }

    private void DrawSplit(Split s, int depth = 0)
    {
        ImGui.SetCursorPosX(depth * 10 + 5);
        ImGui.Text(s.Name);
        ImGui.SameLine();
        if(s.Subsplits.Length != 0)
        {
            ImGui.Text(s.SplitTimes[TimingMethod.RealTime].ToSpeedToolTimerString());
            for(int i = 0; i < s.Subsplits.Length; i++)
                DrawSplit(s.Subsplits[i], depth + 1);
        }
        else
        {
            ImGuiExtensions.EditableTime(s.Name, ref s.SplitTimes.TimeRefFor(TimingMethod.RealTime));
        }
    }
}
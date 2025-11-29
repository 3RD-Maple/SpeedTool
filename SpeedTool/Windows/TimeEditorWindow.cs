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
    private Stack<TimeSpan> times = new();
    private TimeSpan nowTime = TimeSpan.Zero;

    // Just a flag that prevents times from countint on "empty" splits
    private bool timeOkay = true;

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

        nowTime = TimeSpan.Zero;
        timeOkay = true;

        if(ImGui.BeginTable("##Splits", 3, ImGuiTableFlags.BordersH))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Name");
            ImGui.TableNextColumn();
            ImGui.Text("Total");
            ImGui.TableNextColumn();
            ImGui.Text("Segment");
            ImGui.TableNextColumn();
            for(int i = 0; i < splits.Length; i++)
            {
                DrawSplit(splits[i]);
            }
            ImGui.EndTable();
        }

        ImGui.PopFont();

        ImGui.End();
    }

    private void DrawSplit(Split s, int depth = 0)
    {
        ImGui.SetCursorPosX(depth * 10 + 5);
        ImGui.Text(s.Name);
        ImGui.TableNextColumn();
        if(s.Subsplits.Length != 0)
        {
            times.Push(nowTime);
            if(timeOkay)
                ImGui.Text((s.SplitTimes[TimingMethod.RealTime] + nowTime).ToSpeedToolTimerString());
            else
                ImGui.Text("---");
            ImGui.TableNextColumn();
            ImGui.Text(s.SplitTimes[TimingMethod.RealTime].ToSpeedToolTimerString());
            ImGui.TableNextColumn();
            for(int i = 0; i < s.Subsplits.Length; i++)
                DrawSplit(s.Subsplits[i], depth + 1);
            nowTime = times.Pop();
            nowTime += s.SplitTimes[TimingMethod.RealTime];
        }
        else
        {
            if(s.SplitTimes[TimingMethod.RealTime] == TimeSpan.Zero)
                timeOkay = false;
            if(timeOkay)
                ImGui.Text((s.SplitTimes[TimingMethod.RealTime] + nowTime).ToSpeedToolTimerString());
            else
                ImGui.Text("---");
            
            ImGui.TableNextColumn();
            nowTime += s.SplitTimes.TimeRefFor(TimingMethod.RealTime);
            ImGuiExtensions.EditableTime(s.Name, ref s.SplitTimes.TimeRefFor(TimingMethod.RealTime));
            ImGui.TableNextColumn();
        }
    }
}

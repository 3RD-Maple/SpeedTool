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
    public TimeEditorWindow(RunInfo ri) : base(options, new Vector2D<int>(500, 500))
    {
        splits = ri.Splits;
        runInfo = ri;
    }

    private SplitInfo[] splits;
    private RunInfo runInfo;
    private Stack<TimeSpan> times = new();
    private TimeCollection nowTime = new();

    private TimingMethod tm = TimingMethod.RealTime;

    private int i = 0;

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

        nowTime = new();
        i = 0;
        RecalculateTimes();

        Draw("Timing Method", ref tm, (int)TimingMethod.Last);

        ImGui.Text($"{runInfo.GameName} -- {runInfo.CategoryName} ({tm})");

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
                bool isGroup = i < (splits.Length - 1) ? splits[i + 1].Level > splits[i].Level : false;
                DrawSplit(splits[i], isGroup);
            }
            ImGui.EndTable();
        }

        if(ImGui.Button("Save"))
        {
            Platform.Platform.SharedPlatform.SaveRunAsPB(new RunInfo(runInfo.GameName, runInfo.CategoryName, CollectTotalTimes(), runInfo.Splits));
        }
        ImGui.SameLine();
        if(ImGui.Button("Cancel"))
        {
            Close();
        }

        ImGui.PopFont();

        ImGui.End();
    }

    private TimeCollection RecalculateTimes()
    {
        // This is not too good code, probably needs some refactoring
        while(i < splits.Length)
        {
            SplitInfo now = splits[i];
            if(i + 1 == splits.Length)
            {
                nowTime += now.SegmentTime;
                now.TotalTime = nowTime;
                now.DeltaTime = new();
                return nowTime;
            }
            SplitInfo next = splits[i + 1];
            if(next.Level > now.Level)
            {
                i++;
                now.TotalTime = RecalculateTimes();
                now.DeltaTime = new();
                i++;
                continue;
            }
            if(next.Level < now.Level)
            {
                nowTime += now.SegmentTime;
                now.TotalTime = nowTime;
                now.DeltaTime = new();
                return nowTime;
            }

            nowTime += now.SegmentTime;
            now.TotalTime = nowTime;
            now.DeltaTime = new();
            i++;
        }

        return nowTime;
    }

    private TimeCollection CollectTotalTimes()
    {
        TimeCollection ret = new();
        return ret;
    }

    private void DrawSplit(SplitInfo s, bool isGroup)
    {
        ImGui.SetCursorPosX(s.Level * 10 + 5);
        ImGui.Text(s.Name);
        ImGui.TableNextColumn();
        ImGui.Text(s.TotalTime.TimeRefFor(tm).ToSpeedToolTimerString());
        ImGui.TableNextColumn();
        if(isGroup)
            ImGui.Text("--");
        else
            ImGuiExtensions.EditableTime(s.Name, ref s.SegmentTime.TimeRefFor(tm));
        ImGui.TableNextColumn();
    }

    public static bool Draw<T>(string label, ref T value, int maxItems) where T : struct, Enum
    {
        var names = Enum.GetNames<T>().Take(maxItems).ToArray();
        var currentIndex = Array.IndexOf(names, value.ToString());
        
        if (ImGui.Combo(label, ref currentIndex, names, names.Length))
        {
            value = (T)Enum.GetValues<T>().GetValue(currentIndex)!;
            return true;
        }
        
        return false;
    }
}

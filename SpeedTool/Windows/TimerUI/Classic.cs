using System.Numerics;
using ImGuiNET;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Windows.TimerUI;

class ClassicTimerUI : TimerUIBase
{
    public ClassicTimerUI()
    {

    }

    public override void DoUI(ITimerSource source)
    {
        ImGui.BeginTable("##splits_table", 1);
        ISplitsSource splitsSource = new FakeSplitsSource();
        foreach(var split in splitsSource.GetSplits(3))
        {
            ImGui.TableNextColumn();
            if(split.IsCurrent)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, 0xFF00FF00);
            if(split.Level > 0)
                ImGui.SetCursorPosX(split.Level * SPLIT_OFFSET);

            ImGui.Text(split.DisplayString);
        }
        ImGui.EndTable();
        ImGui.Text(source.CurrentTime.ToSpeedToolTimerString());
    }

    class FakeSplitsSource : ISplitsSource
    {
        public SplitDisplayInfo[] GetSplits(int count)
        {
            return new SplitDisplayInfo[] {
                new SplitDisplayInfo("Test 1", false, 0),
                new SplitDisplayInfo("Test 2", true, 1),
                new SplitDisplayInfo("Test 3", false, 0)
            };
        }
    }

    public override void Draw(double dt, ITimerSource source) { }

    private const int SPLIT_OFFSET = 25;
}

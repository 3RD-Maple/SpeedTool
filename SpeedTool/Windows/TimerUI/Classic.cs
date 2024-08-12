using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Windows.TimerUI;

class ClassicTimerUI : TimerUIBase
{
    private ColorSettings ColorsConfig { get; set; } = 
        Configuration.GetSection<ColorSettings>() ?? throw new Exception();
    
    private ClassicUISettings UIConfig { get; set; } = 
        Configuration.GetSection<ClassicUISettings>() ?? throw new Exception();
    
    public ClassicTimerUI()
    {

    }

    public override void DoUI(ITimerSource source)
    {
        ColorsConfig = Configuration.GetSection<ColorSettings>() ?? throw new Exception();
        UIConfig = Configuration.GetSection<ClassicUISettings>() ?? throw new Exception();
        ImGui.BeginTable("##splits_table", 1);
        ISplitsSource splitsSource = new FakeSplitsSource();
        foreach(var split in splitsSource.GetSplits(UIConfig.ShownSplitsCount))
        {
            ImGui.TableNextColumn();
            if (split.IsCurrent)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, Vector4Extensions.ToUint(UIConfig.ActiveSplitColor)); //(uint)UIConfig.ActiveSplitColor.GetHashCode());
            if(split.Level > 0)
                ImGui.SetCursorPosX(split.Level * SPLIT_OFFSET);

            ImGui.TextColored(ColorsConfig.TextColor, split.DisplayString);
        }
        ImGui.EndTable();
        ImGui.TextColored(ColorsConfig.TextColor,source.CurrentTime.ToSpeedToolTimerString());
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

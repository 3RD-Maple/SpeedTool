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

    public override void DoUI(ISplitsSource splits, ITimerSource source)
    {
        ColorsConfig = Configuration.GetSection<ColorSettings>() ?? throw new Exception();
        UIConfig = Configuration.GetSection<ClassicUISettings>() ?? throw new Exception();
        ImGui.BeginTable("##splits_table", 1);
        foreach(var split in splits.GetSplits(UIConfig.ShownSplitsCount))
        {
            ImGui.TableNextColumn();
            if (split.IsCurrent)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, Vector4Extensions.ToUint(UIConfig.ActiveSplitColor));
            if(split.Level > 0)
                ImGui.SetCursorPosX(split.Level * SPLIT_OFFSET);

            ImGui.TextColored(ColorsConfig.TextColor, split.DisplayString);
            if(split.DeltaTimes[TimingMethod.RealTime].TotalMilliseconds != 0)
            {
                ImGui.SameLine();
                ImGui.TextColored(ColorsConfig.TextColor, split.DeltaTimes[TimingMethod.RealTime].ToSpeedToolTimerString());
            }
            else if(split.Times[TimingMethod.RealTime].TotalMilliseconds != 0)
            {
                ImGui.SameLine();
                ImGui.TextColored(ColorsConfig.TextColor, split.Times[TimingMethod.RealTime].ToSpeedToolTimerString());
            }
            ImGui.Separator();
        }
        ImGui.EndTable();
        ImGui.TextColored(ColorsConfig.TextColor,source.CurrentTime.ToSpeedToolTimerString());
    }

    public override void Draw(double dt, ISplitsSource splits, ITimerSource source) { }

    private const int SPLIT_OFFSET = 25;
}

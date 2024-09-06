using System.Numerics;
using ImGuiNET;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;

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
    
    public override void ReloadConfig(object? sender, IConfigurationSection? section)
    {
        if((section as ColorSettings) != null)
            ColorsConfig = (section as ColorSettings)!;

        if((section as ClassicUISettings) != null)
            UIConfig = (section as ClassicUISettings)!;
    }

    public override void DoUI(ISplitsSource splits, ITimerSource source)
    {
        ColorsConfig = Configuration.GetSection<ColorSettings>() ?? throw new Exception();
        UIConfig = Configuration.GetSection<ClassicUISettings>() ?? throw new Exception();
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, 0);
        ImGui.PushStyleColor(ImGuiCol.TableBorderLight, new Vector4(0.349f, 0.341f, 0.384f, 1.0f));
        ImGui.BeginTable("##splits_table", 1, ImGuiTableFlags.BordersInner);
        var gotSplits = splits.GetSplits(UIConfig.ShownSplitsCount).ToArray();
        for(int i = 0; i < gotSplits.Count(); i++)
        {
            var split = gotSplits[i];
            var isLast = i == (gotSplits.Length - 1) && split.Times[DisplayTimingMethod].Ticks != 0;

            var timeText = isLast ? GetTimeStringForLastSplit(ref split) : GetTimeString(ref split, source);
            var timeTextLen = timeText.Item2 == "" ? 0 : ImGui.CalcTextSize(timeText.Item2).X;
            var splitOffset = split.Level * SPLIT_OFFSET;
            var splitText = ImGuiExtensions.ShortenStringForWidth((int)(ImGui.GetWindowSize().X - timeTextLen - splitOffset - 35), split.DisplayString);

            ImGui.TableNextColumn();
            if (split.IsCurrent)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, Vector4Extensions.ToUint(UIConfig.ActiveSplitColor));
            else if(i % 2 != 0)
            {
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, Vector4Extensions.ToUint(new Vector4(0.2f, 0.2f, 0.2f, 1.0f)));
            }
            if(split.Level > 0)
                ImGui.SetCursorPosX(split.Level * SPLIT_OFFSET);

            ImGui.TextColored(ColorsConfig.TextColor, splitText);
            ImGui.SameLine();
            TextRightAlign(timeText.Item1, timeText.Item2);
            ImGui.TableNextRow();
        }
        if(DisplayTimingMethod != TimingMethod.RealTime && UIConfig.ShowRTA)
        {
            ImGui.TableNextColumn();
            TextRightAlign(ColorsConfig.TextColor, "RTA: " + Platform.Platform.SharedPlatform.GetTimerFor(TimingMethod.RealTime).CurrentTime.ToSpeedToolTimerString());
        }
        ImGui.EndTable();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(5);
    }

    private (Vector4, string) GetTimeString(ref SplitDisplayInfo displayInfo, ITimerSource source)
    {
        if(displayInfo.IsCurrent)
        {
            return (ColorsConfig.TextColor, source.CurrentTime.ToSpeedToolTimerString());
        }
        else if(displayInfo.DeltaTimes[DisplayTimingMethod].TotalMilliseconds != 0)
        {
            bool negative = displayInfo.DeltaTimes[DisplayTimingMethod].Ticks < 0;
            Vector4 col = negative ? ColorsConfig.AheadColor : ColorsConfig.BehindColor;
            ImGui.SameLine();
            if(negative)
                return (ColorsConfig.AheadColor, "-" + displayInfo.DeltaTimes[DisplayTimingMethod].ToSpeedToolTimerString());
            else
                return (ColorsConfig.BehindColor, "+" + displayInfo.DeltaTimes[DisplayTimingMethod].ToSpeedToolTimerString());
        }
        else if(displayInfo.Times[DisplayTimingMethod].TotalMilliseconds != 0)
        {
            return (ColorsConfig.TextColor, displayInfo.Times[DisplayTimingMethod].ToSpeedToolTimerString());
        }

        return (ColorsConfig.TextColor, "");
    }

    private (Vector4, string) GetTimeStringForLastSplit(ref SplitDisplayInfo displayInfo)
    {
        var color = IsSplitAhead(ref displayInfo, DisplayTimingMethod) ? ColorsConfig.AheadColor : ColorsConfig.BehindColor;

        return (color, displayInfo.DeltaTimes[DisplayTimingMethod].ToSpeedtoolDTString() + " " + displayInfo.Times[DisplayTimingMethod].ToSpeedToolTimerString());
    }

    private bool IsSplitAhead(ref SplitDisplayInfo displayInfo, TimingMethod timingMethod)
    {
        return displayInfo.DeltaTimes[timingMethod].Ticks < 0;
    }

    private static void TextRightAlign(Vector4 color, string text)
    {
        if(text == "")
            return;
        var sz = ImGui.CalcTextSize(text).X;
        var posX = ImGui.GetWindowWidth() - sz - 10;
        ImGui.SetCursorPosX(posX);
        ImGui.TextColored(color, text);
    }

    public override void Draw(double dt, ISplitsSource splits, ITimerSource source) { }

    private const int SPLIT_OFFSET = 25;
}

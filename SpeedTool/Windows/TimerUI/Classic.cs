using System.Numerics;
using ImGuiNET;
using Silk.NET.Windowing;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows.TimerUI;

class ClassicTimerUI : TimerUIBase
{
    private bool ConfigChangePending { get; set; } = false;
    private DateTime ConfigChangePendingSince { get; set; } = DateTime.MinValue;
    private ColorSettings ColorsConfig { get; set; } = Configuration.GetSection<ColorSettings>();

    private ClassicUISettings UIConfig { get; set; } = Configuration.GetSection<ClassicUISettings>();

    public ClassicTimerUI()
    {

    }

    public override WindowBorder DesiredBorder => WindowBorder.Resizable;

    public override Vector2 DesiredSize => UIConfig.DesiredSizes;

    public override void Resizing(Vector2 newSizes)
    {
        if(newSizes == UIConfig.DesiredSizes)
            return;

        UIConfig.DesiredSizes = newSizes;
        ConfigChangePendingSince = DateTime.Now;
        ConfigChangePending = true;
        Configuration.SetSection(UIConfig);
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
        CheckAndSaveConfig();
        ColorsConfig = Configuration.GetSection<ColorSettings>();
        UIConfig = Configuration.GetSection<ClassicUISettings>();
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

            var timeText = isLast ? GetTimeStringForLastSplit(ref split) : GetTimeString(ref split, source, splits);
            var timeTextLen = timeText.Item2 == "" ? 0 : ImGui.CalcTextSize(timeText.Item2).X;
            var splitOffset = split.Level * SPLIT_OFFSET;
            var splitText = ImGuiExtensions.ShortenStringForWidth((int)(ImGui.GetWindowSize().X - timeTextLen - splitOffset - 35), split.DisplayString);

            ImGui.TableNextColumn();
            if (split.IsCurrent)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, Vector4Extensions.ToUint(UIConfig.ActiveSplitColor));
            else if(UIConfig.AlternateSplitBackround && i % 2 != 0)
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

    private (Vector4, string) GetTimeString(ref SplitDisplayInfo displayInfo, ITimerSource source, ISplitsSource splits)
    {
        if(displayInfo.IsCurrent)
        {
            if(displayInfo.PBSplit != null)
            {
                var currentSeg = source.CurrentTime;
                if(splits.PreviousSplit != null)
                    currentSeg -= splits.PreviousSplit.Times[DisplayTimingMethod];
                if(currentSeg > displayInfo.PBSplit.SegmentTime[DisplayTimingMethod])
                {
                    return (ColorsConfig.BehindColor, source.CurrentTime.ToSpeedToolTimerString());
                }
            }
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
        else if(displayInfo.PBSplit != null && displayInfo.PBSplit.TotalTime[DisplayTimingMethod].Ticks != 0)
        {
            return (ColorsConfig.TextColor, displayInfo.PBSplit.TotalTime[DisplayTimingMethod].ToSpeedToolTimerString());
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

    private void CheckAndSaveConfig()
    {
        // FIXME:
        //  Once the user starts resizing the window, the resize message is spammed. If I were to save
        //  new sizes on every resize message, it would lead to config being rewritten too many times.
        //  To prevent too mane file operations, only save it 5 secods after the last resize message.
        //  This is a hack but will do for now.
        if(ConfigChangePending && (DateTime.Now - ConfigChangePendingSince) > TimeSpan.FromSeconds(5))
        {
            ConfigChangePending = false;
            ConfigChangePendingSince = DateTime.MinValue;
            Configuration.SetSection(UIConfig);
        }
    }

    public override void Draw(double dt, ISplitsSource splits, ITimerSource source) { }

    private const int SPLIT_OFFSET = 25;
}

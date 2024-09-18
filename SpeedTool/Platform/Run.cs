using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.Platform;

public class Run : ISplitsSource
{
    public Run(Game game, Category cat, RunInfo? comparisonRun)
    {
        this.game = game;
        comparison = comparisonRun;
        category = cat;
        controller = new SplitsController(cat, comparisonRun);
    }

    public bool Started { get; private set; }
    public bool IsFinished { get; private set; }

    public ITimerSource Timer
    {
        get
        {
            return timer;
        }
    }

    public SplitDisplayInfo CurrentSplit => controller.CurrentSplit;

    public void Split()
    {
        // No splitting when paused!
        if(timer.CurrentState == TimerState.Paused)
            return;

        if(!Started)
        {
            if(timer.CurrentState != TimerState.NoState)
            {
                Platform.SharedPlatform.ReloadRun();
                timer.Reset();
                IsFinished = false;
                return;
            }
            Started = true;
            controller.Start();
            timer.Reset();
            timer.Start();
            return;
        }

        if(!controller.NextSplit(Platform.SharedPlatform.GetCurrentTimes()))
        {
            timer.Stop();
            Started = false;
            IsFinished = true;
            SaveRun();
        }
    }

    public void Pause()
    {
        if(Started)
            timer.Pause();
    }

    public void SkipSplit()
    {
        if(!Started || IsFinished)
        {
            return;
        }

        controller.SkipSplit();
    }

    public IEnumerable<SplitDisplayInfo> GetSplits(int count)
    {
        return controller.GetSplits(count);
    }

    public RunInfo GetRunInfo()
    {
        if(game == null)
            return new RunInfo("unnamed", "unnamed", controller.AllSplits.Last().Times, controller.AllSplits.Select(x => x.ToSplitInfo()).ToArray());
        return new RunInfo(game!.Name, Platform.SharedPlatform.CurrentCategory!.Name, controller.AllSplits.Last().Times, controller.AllSplits.Select(x => x.ToSplitInfo()).ToArray());
    }

    private void SaveRun()
    {
        var tm = game.DefaultTimingMethod;
        if(comparison == null || controller.AllSplits.Last().Times[tm] < comparison!.TotalTimes[tm])
        {
            Platform.SharedPlatform.SaveRunAsPB(GetRunInfo());
        }
    }

    public void UndoSplit()
    {
        if(!Started || IsFinished)
            return;

        controller.UndoSplit();
    }

    private RunInfo? comparison;
    private Game game;
    private Category category;

    SplitsController controller;

    public SplitDisplayInfo? PreviousSplit => controller.PreviousSplit;

    const int weight = 75;

    BasicTimer timer = new();
}

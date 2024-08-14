using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Platform;

public class Run : ISplitsSource
{
    public Run(Game game, Split[] splits, Run? comparisonRun)
    {
        this.splits = splits;
        FlattenSplits();
        comparison = comparisonRun;
    }

    public bool Started { get; private set; }
    public ITimerSource Timer
    {
        get
        {
            return timer;
        }
    }

    public SplitDisplayInfo CurrentSplit => new SplitDisplayInfo(flattened[currentSplit].DisplayString, true, flattened[currentSplit].Level);

    public void Split()
    {
        if(!Started)
        {
            if(timer.CurrentState != TimerState.NoState)
            {
                timer.Reset();
                currentSplit = 0;
                return;
            }
            Started = true;
            currentSplit = -1;
            NextSplit();
            flattened[currentSplit].IsCurrent = true;
            timer.Reset();
            timer.Start();
            return;
        }

        NextSplit();
        if(currentSplit >= flattened.Length)
        {
            timer.Stop();
            Started = false;
            currentSplit--;
            return;
        }
        flattened[currentSplit].IsCurrent = true;
    }

    public void SkipSplit()
    {
        if(!Started)
        {
            return;
        }

        flattened[currentSplit].IsCurrent = false;
        NextSplitNoUpdate();
        if(currentSplit >= flattened.Length)
        {
            timer.Stop();
            Started = false;
            currentSplit--;
            return;
        }
        flattened[currentSplit].IsCurrent = true;
    }

    private void NextSplitNoUpdate()
    {
        // Write split time
        currentSplit++;

        // Roll over parent splitts
        while(infoStack.Count > 0 && infoStack.Peek().Level >= flattened[currentSplit].Level)
        {
            var p = infoStack.Pop();
        }

        // Roll over to the first actual split in the tree
        while(NextFlatSplit != null && CurrentFlatSplit.Level < NextFlatSplit.Value.Level)
        {
            infoStack.Push(flattened[currentSplit]);
            currentSplit++;
        }

        // If split has subsplits, go to next split
        if(NextFlatSplit != null && NextFlatSplit.Value.Level > CurrentFlatSplit.Level)
        {
            SkipSplit();
        }
    }

    private void FlattenSplits()
    {
        List<SplitDisplayInfo> f = new();
        List<int> zero = new();
        foreach(var split in splits)
        {
            zero.Add(f.Count);
            f.AddRange(split.Flatten());
        }

        flattened = f.ToArray();
        zeroLevelSplits = zero.ToArray();
    }

    int currentSplit = 0;

    private Split[] splits;

    private SplitDisplayInfo[] flattened = [];
    private int[] zeroLevelSplits = [];

    public IEnumerable<SplitDisplayInfo> GetSplits(int count)
    {
        // Honestly, I wrote this function when I was high on sleep deprevation and eye disease,
        // so it might be difficult to understand the hell is going on here.
        // I'm trying to explain it as an aftermath with clear head, so don't mind me

        // This function has a `wieght` parameter to decide how to value splits from each side.
        // TODO: This weight feature doesn't really work well, it should probably be changed to something else
        var currentLevelSplits = GetCurrentLevelSplits();
        var posInCurrentLevel = currentLevelSplits.IndexOf(CurrentFlatSplit);

        // If on level 0, just get enough splits to display
        if(CurrentFlatSplit.Level == 0)
        {
            return currentLevelSplits.TakeAtPosWeighted(count, posInCurrentLevel, weight);
        }

        // Always display splits tree on top
        var topmostSplits = GetTopmostSplits();
        var topmostCount = Math.Min(count - 1, topmostSplits.Count);

        // If we have enough tree to fill in the requested space, do that
        if(topmostCount == count - 1)
            return topmostSplits.TakeLast(topmostCount).Append(CurrentFlatSplit);

        var currentLevelCount = Math.Min(currentLevelSplits.Count, count - topmostCount);

        // If tree + current level fits the space, do that
        if(currentLevelCount + topmostCount >= count)
        {
            return topmostSplits.TakeLast(topmostCount).Concat(currentLevelSplits.TakeAtPosWeighted(currentLevelCount, posInCurrentLevel, weight));
        }

        var middle = topmostSplits.TakeLast(topmostCount).Concat(currentLevelSplits.TakeAtPosWeighted(currentLevelCount, posInCurrentLevel, weight));

        var zeroLevelCount = count - currentLevelCount - topmostCount;

        zeroLevelSplits.Select(x => flattened[x]).TakeAtPosWeighted(zeroLevelCount, 1, weight);

        // Figure out next and previous splits to fit the space
        List<SplitDisplayInfo> prev = new();
        List<SplitDisplayInfo> next = new();
        int i = 0;
        while(i < zeroLevelSplits.Length && zeroLevelSplits[i] < currentSplit)
        {
            if(zeroLevelSplits[i] == currentSplit)
            {
                i++;
                continue;
            }
            prev.Add(flattened[zeroLevelSplits[i]]);
            i++;
        }
        while(i < zeroLevelSplits.Length)
        {
            if(zeroLevelSplits[i] == currentSplit)
            {
                i++;
                continue;
            }
            next.Add(flattened[zeroLevelSplits[i]]);
            i++;
        }

        // Figure out how many splits to take from the left and from the right
        var wantRight = (int)(weight / 100.0) * zeroLevelCount;
        wantRight = Math.Min(next.Count, wantRight);

        var wantLeft = zeroLevelCount - wantRight;
        wantLeft = Math.Min(wantLeft, prev.Count);

        return prev.TakeLast(wantLeft).Concat(middle).Concat(next.Take(wantRight));
    }

    private void NextSplit()
    {
        // Write split time
        if(currentSplit >= 0)
        {
            flattened[currentSplit].IsCurrent = false;
            flattened[currentSplit].Times[TimingMethod.RealTime] = timer.CurrentTime;
            if(comparison != null)
                flattened[currentSplit].DeltaTimes[TimingMethod.RealTime] = flattened[currentSplit].Times[TimingMethod.RealTime] - comparison.flattened[currentSplit].Times[TimingMethod.RealTime];
        }

        currentSplit++;

        // Roll over parent splitts and write times for them
        while(infoStack.Count > 0 && infoStack.Peek().Level >= flattened[currentSplit].Level)
        {
            var p = infoStack.Pop();
            p.Times[TimingMethod.RealTime] = timer.CurrentTime;
            if(comparison != null)
                p.DeltaTimes[TimingMethod.RealTime] = flattened[currentSplit].Times[TimingMethod.RealTime] - comparison.flattened[currentSplit].Times[TimingMethod.RealTime];
        }

        // Roll over to the first actual split in the tree
        while(NextFlatSplit != null && CurrentFlatSplit.Level < NextFlatSplit.Value.Level)
        {
            infoStack.Push(flattened[currentSplit]);
            currentSplit++;
        }

        // If split has subsplits, go to next split
        if(NextFlatSplit != null && NextFlatSplit.Value.Level > CurrentFlatSplit.Level)
        {
            NextSplit();
        }
    }

    private List<SplitDisplayInfo> GetCurrentLevelSplits()
    {
        var begin = FirstSubsplitPos();
        var end = LastSubsplitPos();

        List<SplitDisplayInfo> infos = new();

        for(int i = begin; i <= end; i++)
        {
            if(flattened[i].Level == CurrentFlatSplit.Level)
                infos.Add(flattened[i]);
        }

        return infos;
    }

    private List<SplitDisplayInfo> GetTopmostSplits()
    {
        var list = infoStack.ToList();
        list.Reverse();
        return list;
    }

    private int FirstSubsplitPos()
    {
        for(int i = currentSplit; i >= 0; i--)
        {
            if(flattened[i].Level < CurrentFlatSplit.Level)
                return i + 1;
        }
        return 0;
    }

    private int LastSubsplitPos()
    {
        for(int i = currentSplit; i < flattened.Length; i++)
        {
            if(flattened[i].Level < CurrentFlatSplit.Level)
                return i - 1;
        }
        return flattened.Length - 1;
    }

    private Run? comparison;

    private Stack<SplitDisplayInfo> infoStack = new();

    private SplitDisplayInfo? NextFlatSplit => currentSplit >= flattened.Length - 1 ? null : flattened[currentSplit + 1];
    private SplitDisplayInfo CurrentFlatSplit => flattened[currentSplit];
    private SplitDisplayInfo? PreviousFlatSplit => currentSplit <= 0 ? null : flattened[currentSplit - 1];

    const int weight = 75;

    BasicTimer timer = new();
}

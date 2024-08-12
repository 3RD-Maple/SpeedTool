using SpeedTool.Splits;
using SpeedTool.Timer;

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

    public SplitDisplayInfo[] GetSplits(int count)
    {
        if(!Started)
        {
            return zeroLevelSplits.Take(count).Select(x => flattened[x]).ToArray();
        }

        SplitDisplayInfo[] ret = new SplitDisplayInfo[count];
        var currentLevelSplits = GetCurrentLevelSplits();
        var posInCurrentLevel = currentLevelSplits.IndexOf(CurrentFlatSplit);

        var topSplitCount = infoStack.Count;

        var currentLevelSplitsCount = currentLevelSplits.Count;
        var nextSplitsCount = currentLevelSplitsCount - posInCurrentLevel;
        var prevSplitsCount = posInCurrentLevel;

        int pos = 0;

        // Propagate top-level splits
        foreach(var spl in infoStack)
        {
            ret[infoStack.Count - pos - 1] = spl;
            pos++;
            if(pos == count - 1)
                break;
        }

        // Populate current-level splits
        if(pos + currentLevelSplitsCount >= count)
        {
            // If we have enough splits to populate requested amount, do just that
            // If can populate with "next", do that
            if(pos + nextSplitsCount >= count)
            {
                for(int i = 0; i < count - pos; i++)
                    ret[pos + i] = currentLevelSplits[posInCurrentLevel + i];
                return ret;
            }
            else // Otherwise, populate with "back" splits, then "next"
            {
                var neededPreviousSplits = count - pos - nextSplitsCount;
                for(int i = neededPreviousSplits; i > 0; i--)
                {
                    ret[pos] = currentLevelSplits[posInCurrentLevel - i];
                    pos++;
                }
                // Fill the rest with "next" splits
                for(int i = 0; i < count - pos; i++)
                    ret[pos + i] = currentLevelSplits[posInCurrentLevel + i];
                return ret;
            }
        }
        else // // We need to populate with zero level splits too!
        {
            // First, populate with current level splits
            ret = GetSplits(pos + currentLevelSplitsCount);

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
            var prevCount = Math.Min(count - pos, prev.Count);
            var nextCount = Math.Max(count - pos - prevCount, next.Count);
            return prev.TakeLast(prevCount).Concat(ret.Take(pos)).Concat(next.Take(nextCount)).ToArray();
        }
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

    private int FirstSubsplitPos()
    {
        for(int i = currentSplit; i > 0; i--)
        {
            if(flattened[i].Level < CurrentFlatSplit.Level)
                return i + 1;
        }
        return currentSplit;
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

    BasicTimer timer = new();
}

using System.Diagnostics.CodeAnalysis;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Splits;

public sealed class SplitsController
{
    public SplitsController(Category category, RunInfo? comparisonRun)
    {
        FlattenSplits(category);

        if(comparisonRun != null && comparisonRun.Splits.Length == flattened.Length)
        {
            for(int i = 0; i < comparisonRun.Splits.Length; i++)
                flattened[i].PBSplit = comparisonRun.Splits[i];
            comparison = comparisonRun;
        }
    }

    public SplitDisplayInfo CurrentSplit => flattened[Math.Max(0, currentSplitId)];

    public SplitDisplayInfo[] AllSplits => flattened;

    public SplitDisplayInfo? PreviousSplit
    {
        get
        {
            if(PreviousFlatSplit == null)
                return null;

            int i = currentSplitId;
            while(i > 0)
            {
                i--;

                if(flattened[i + 1].Level <= flattened[i].Level)
                {
                    break;
                }
            }

            if(i >= 0)
                return flattened[i];

            return null;
        }
    }

    private SplitDisplayInfo[] flattened;

    [MemberNotNull(nameof(flattened))]
    [MemberNotNull(nameof(zeroLevelSplits))]
    private void FlattenSplits(Category category)
    {
        List<SplitDisplayInfo> f = new();
        List<int> zero = new();
        foreach(var split in category.Splits)
        {
            zero.Add(f.Count);
            f.AddRange(split.Flatten());
        }

        flattened = f.ToArray();
        zeroLevelSplits = zero.ToArray();
    }

    public void Start()
    {
        currentSplitId = -1;
        NextSplit(new());
    }

    public bool NextSplit(TimeCollection times)
    {
        // Write split time
        if(currentSplitId >= 0)
        {
            flattened[currentSplitId].IsCurrent = false;
            flattened[currentSplitId].Times = times;
            if(comparison != null)
                flattened[currentSplitId].DeltaTimes = flattened[currentSplitId].Times - comparison.Splits[currentSplitId].TotalTime;
            if(PreviousSplit != null)
                flattened[currentSplitId].SegmentTimes = flattened[currentSplitId].Times - PreviousSplit.Times;
            else
                flattened[currentSplitId].SegmentTimes = flattened[currentSplitId].Times;
        }

        currentSplitId++;
        if(currentSplitId >= flattened.Length)
        {
            currentSplitId--;
            return false;
        }

        // Roll over parent splitts and write times for them
        while(infoStack.Count > 0 && infoStack.Peek().Split.Level >= flattened[currentSplitId].Level)
        {
            var p = infoStack.Pop();
            for(int i = 0; i < (int)TimingMethod.Last; i++)
            {
                var tm = (TimingMethod)i; 
                p.Split.Times[tm] = Platform.Platform.SharedPlatform.GetTimerFor(tm).CurrentTime;
            }
            if(comparison != null)
                p.Split.DeltaTimes = flattened[currentSplitId].Times - comparison.Splits[currentSplitId].TotalTime;
        }

        // Roll over to the first actual split in the tree
        while(NextFlatSplit != null && CurrentFlatSplit.Level < NextFlatSplit.Level)
        {
            infoStack.Push(new(flattened[currentSplitId], Platform.Platform.SharedPlatform.GetCurrentTimes()));
            currentSplitId++;
        }

        // If split has subsplits, go to next split
        if(NextFlatSplit != null && NextFlatSplit.Level > CurrentFlatSplit.Level)
        {
            if(!NextSplit(times))
                return false;
        }

        flattened[currentSplitId].IsCurrent = true;
        return true;
    }

    public void UndoSplit()
    {
        while(PreviousFlatSplit != null)
        {
            flattened[currentSplitId].IsCurrent = false;
            currentSplitId--;
            flattened[currentSplitId].Times.Reset();
            flattened[currentSplitId].DeltaTimes.Reset();

            if(NextFlatSplit!.Level <= CurrentFlatSplit.Level)
            {
                break;
            }
        }
        flattened[currentSplitId].IsCurrent = true;
        ResetInfoStack();

        // Reset infoStack somehow
        foreach(var i in infoStack.AsEnumerable())
        {
            i.Split.Times.Reset();
            i.Split.DeltaTimes.Reset();
        }
    }

    public void SkipSplit()
    {
        if(currentSplitId == flattened.Length - 1)
            return;
        flattened[currentSplitId].IsCurrent = false;
        // Write split time
        currentSplitId++;

        // Roll over parent splitts
        while(infoStack.Count > 0 && infoStack.Peek().Split.Level >= flattened[currentSplitId].Level)
        {
            var p = infoStack.Pop();
        }

        // Roll over to the first actual split in the tree
        while(NextFlatSplit != null && CurrentFlatSplit.Level < NextFlatSplit.Level)
        {
            infoStack.Push(new(flattened[currentSplitId], Platform.Platform.SharedPlatform.GetCurrentTimes()));
            currentSplitId++;
        }

        // If split has subsplits, go to next split
        if(NextFlatSplit != null && NextFlatSplit.Level > CurrentFlatSplit.Level)
        {
            SkipSplit();
        }

        flattened[currentSplitId].IsCurrent = true;
    }

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
        while(i < zeroLevelSplits.Length && zeroLevelSplits[i] < currentSplitId)
        {
            if(zeroLevelSplits[i] == currentSplitId)
            {
                i++;
                continue;
            }
            prev.Add(flattened[zeroLevelSplits[i]]);
            i++;
        }
        prev = prev.Take(prev.Count - 1).ToList();
        while(i < zeroLevelSplits.Length)
        {
            if(zeroLevelSplits[i] == currentSplitId)
            {
                i++;
                continue;
            }
            next.Add(flattened[zeroLevelSplits[i]]);
            i++;
        }

        // Figure out how many splits to take from the left and from the right
        var wantRight = (int)(weight / 100.0 * zeroLevelCount);
        wantRight = Math.Min(next.Count, wantRight);

        var wantLeft = zeroLevelCount - wantRight;
        wantLeft = Math.Min(wantLeft, prev.Count);

        return prev.TakeLast(wantLeft).Concat(middle).Concat(next.Take(wantRight));
    }

    private void ResetInfoStack()
    {
        var upTo = currentSplitId;
        currentSplitId = 0;
        infoStack = new();
        while(currentSplitId != upTo)
        {
            SkipSplit();
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
        return infoStack.Reverse().Select(x => x.Split).ToList();
    }

    private int FirstSubsplitPos()
    {
        for(int i = currentSplitId; i >= 0; i--)
        {
            if(flattened[i].Level < CurrentFlatSplit.Level)
                return i + 1;
        }
        return 0;
    }

    private int LastSubsplitPos()
    {
        if(currentSplitId < 0)
            return 0;
        for(int i = currentSplitId; i < flattened.Length; i++)
        {
            if(flattened[i].Level < CurrentFlatSplit.Level)
                return i - 1;
        }
        return flattened.Length - 1;
    }

    class SplitStackInfo
    {
        public SplitStackInfo(SplitDisplayInfo s, TimeCollection times)
        {
            Split = s;
            StartTimes = times;
        }

        public SplitDisplayInfo Split;
        public TimeCollection StartTimes;
    }

    private Stack<SplitStackInfo> infoStack = new();

    private SplitDisplayInfo? NextFlatSplit => currentSplitId >= flattened.Length - 1 ? null : flattened[currentSplitId + 1];
    private SplitDisplayInfo CurrentFlatSplit => flattened[Math.Max(0, currentSplitId)];
    private SplitDisplayInfo? PreviousFlatSplit => currentSplitId <= 0 ? null : flattened[currentSplitId - 1];

    int currentSplitId = 0;

    private RunInfo? comparison;

    private int[] zeroLevelSplits;

    const int weight = 75;
}

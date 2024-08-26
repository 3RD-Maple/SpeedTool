namespace SpeedTool.Splits;

public class NullSplitsSource : ISplitsSource
{
    public SplitDisplayInfo CurrentSplit => split;

    public SplitDisplayInfo? PreviousSplit => null;

    public IEnumerable<SplitDisplayInfo> GetSplits(int count)
    {
        return [split];
    }

    private SplitDisplayInfo split = new SplitDisplayInfo("No Game", false, 0);
}

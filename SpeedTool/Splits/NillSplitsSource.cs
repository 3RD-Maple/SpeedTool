namespace SpeedTool.Splits;

public class NullSplitsSource : ISplitsSource
{
    public SplitDisplayInfo CurrentSplit => new SplitDisplayInfo("", true, 0);

    public IEnumerable<SplitDisplayInfo> GetSplits(int count)
    {
        return [];
    }
}

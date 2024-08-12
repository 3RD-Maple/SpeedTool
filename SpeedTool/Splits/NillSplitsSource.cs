namespace SpeedTool.Splits;

public class NullSplitsSource : ISplitsSource
{
    public SplitDisplayInfo CurrentSplit => new SplitDisplayInfo("", true, 0);

    public SplitDisplayInfo[] GetSplits(int count)
    {
        return [];
    }
}

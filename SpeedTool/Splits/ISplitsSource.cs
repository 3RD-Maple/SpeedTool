namespace SpeedTool.Splits;

public interface ISplitsSource
{
    /// <summary>
    /// Get `count` splits for display purposes
    /// </summary>
    /// <param name="count">Amount of splits to display</param>
    /// <returns>Up to `count` splits to display. Note that the actual amount of splits can be less than requested!</returns>
    SplitDisplayInfo[] GetSplits(int count);

    /// <summary>
    /// Get currently active split
    /// </summary>
    SplitDisplayInfo CurrentSplit { get; }
}
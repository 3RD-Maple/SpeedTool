namespace SpeedTool.Splits;

interface ISplitsSource
{
    /// <summary>
    /// Get `count` splits for display purposes
    /// </summary>
    /// <param name="count">Amount of splits to display</param>
    /// <returns>`count` splits to display</returns>
    SplitDisplayInfo[] GetSplits(int count);
}
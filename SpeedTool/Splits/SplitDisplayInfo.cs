namespace SpeedTool.Splits;

struct SplitDisplayInfo
{
    public SplitDisplayInfo(string name, bool active, int level)
    {
        DisplayString = name;
        IsCurrent = active;
        Level = level;
    }

    /// <summary>
    /// Is this a split that's currently being run
    /// </summary>
    public bool IsCurrent { get; private set; }

    /// <summary>
    /// Split's sublevel in the splits tree
    /// </summary>
    public int Level { get; private set; }

    public string DisplayString { get; private set; }
}
namespace SpeedTool.Splits;

public struct SplitDisplayInfo
{
    public SplitDisplayInfo(string name, bool active, int level)
    {
        DisplayString = name;
        IsCurrent = active;
        Level = level;
    }

    public SplitDisplayInfo(Split s)
    {
        DisplayString = s.Name;
        IsCurrent = false;
        Level = 0;
    }

    /// <summary>
    /// Is this a split that's currently being run
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Split's sublevel in the splits tree
    /// </summary>
    public int Level { get; private set; }

    public string DisplayString { get; private set; }

    public TimeCollection DeltaTimes = new();

    public TimeCollection Times = new();
}

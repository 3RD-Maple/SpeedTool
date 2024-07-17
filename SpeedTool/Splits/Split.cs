namespace SpeedTool.Splits;

struct Split
{
    public Split()
    {
        Name = "";
        Subsplits = new List<Split>();
        SplitTimes = new();
    }

    public string Name;

    public List<Split> Subsplits;

    public TimeCollection SplitTimes;
}

namespace SpeedTool.Splits;

struct Split
{
    public Split()
    {
        Name = "";
        Subsplits = new List<Split>();
        SplitTimes = new SplitTimes();
    }

    public string Name;

    public List<Split> Subsplits;

    public SplitTimes SplitTimes;
}

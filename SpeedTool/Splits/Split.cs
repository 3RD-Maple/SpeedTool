using SpeedTool.Util;

namespace SpeedTool.Splits;

public class Split
{
    public Split()
    {
        Name = "";
        Subsplits = new Split[0];
        SplitTimes = new();
    }

    public Split(string name) : this()
    {
        Name = name;
    }

    public string Name;

    public Split[] Subsplits;

    public TimeCollection SplitTimes;

    public void AddSubsplit(Split split)
    {
        Subsplits = Subsplits.Append(split).ToArray();
    }

    public void InsertSplit(int idx, Split split)
    {
        Subsplits = Subsplits.InsertAt(idx, split);
    }
}

namespace SpeedTool.Splits;

public class RunInfo
{
    public RunInfo(string name, string cat, TimeCollection times, SplitInfo[] infos)
    {
        CategoryName = cat;
        GameName = name;
        TotalTimes = times;
        Splits = infos;
    }

    public string CategoryName { get; set; }
    public string GameName { get; set; }
    public TimeCollection TotalTimes { get; set; }

    public SplitInfo[] Splits { get; set; }
}

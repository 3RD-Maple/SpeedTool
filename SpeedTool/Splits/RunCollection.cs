namespace SpeedTool.Splits;

// This will probably be used later on in life
class RunCollection
{
    public RunCollection() { }

    public RunInfo BestRun
    {
        get
        {
            if(!HasRuns)
                throw new Exception("Collection was empty");
            return runs.OrderBy(x => x.Times[Timer.TimingMethod.RealTime]).FirstOrDefault()!;
        }
    }

    public bool HasRuns
    {
        get
        {
            return runs.Count > 0;
        }
    }

    public void AddRun(RunInfo info)
    {
        runs.Add(info);
    }

    private List<RunInfo> runs = new();
}

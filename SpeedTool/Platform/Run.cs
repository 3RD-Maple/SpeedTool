using SpeedTool.Splits;

namespace SpeedTool.Platform;

public class Run : ISplitsSource
{
    public Run(Game game, Split[] splits)
    {
        this.splits = splits;
        FlattenSplits();
    }

    public bool Started { get; private set; }

    public void Split()
    {
        if(!Started)
        {
            Started = true;
            flattened[0].IsCurrent = true;
            return;
        }

        flattened[currentSplit].IsCurrent = false;
        currentSplit++;
        if(currentSplit >= flattened.Length)
        {
            // Finish run
        }
        flattened[currentSplit].IsCurrent = true;
    }

    private void FlattenSplits()
    {
        List<SplitDisplayInfo> f = new();
        foreach(var split in splits)
        {
            f.AddRange(split.Flatten());
        }

        flattened = f.ToArray();
    }

    int currentSplit = 0;

    private Split[] splits;

    private SplitDisplayInfo[] flattened = [];

    public SplitDisplayInfo[] GetSplits(int count)
    {
        SplitDisplayInfo[] ret = new SplitDisplayInfo[count];
        for(int i = 0; i < count; i++)
        {
            ret[i] = flattened[i];
        }
        return ret;
    }
}

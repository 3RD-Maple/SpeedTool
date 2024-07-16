using SpeedTool.Timer;

namespace SpeedTool.Splits;

struct SplitTimes
{
    public SplitTimes()
    {
        times = new Dictionary<TimingMethod, TimeSpan>();
    }

    TimeSpan TimeForTimingMethod(TimingMethod method)
    {
        // TODO: Will be implemented later
        return new TimeSpan();
    }

    private Dictionary<TimingMethod, TimeSpan> times;
}
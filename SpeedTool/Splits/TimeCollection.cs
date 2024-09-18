using SpeedTool.Timer;

namespace SpeedTool.Splits;

public struct TimeCollection
{
    public TimeCollection()
    {

    }

    public TimeSpan this[TimingMethod timingMethod]
    {
        get
        {
            return spans[(int)timingMethod];
        }
        set
        {
            spans[(int)timingMethod] = value;
        }
    }

    public static TimeCollection operator-(TimeCollection a, TimeCollection b)
    {
        var res = new TimeCollection();
        for(int i = 0; i < TIMES_COUNT; i++)
        {
            var TM = (TimingMethod)i;
            res[TM] = a[TM] - b[TM];
        }

        return res;
    }

    public void Reset()
    {
        for(int i = 0; i < TIMES_COUNT; i++)
        {
            spans[i] = new TimeSpan(0);
        }
    }

    private TimeSpan[] spans = new TimeSpan[TIMES_COUNT];

    const int TIMES_COUNT = 6;
}

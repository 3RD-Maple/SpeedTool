using System.Collections;
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

    private TimeSpan[] spans = new TimeSpan[6];
}

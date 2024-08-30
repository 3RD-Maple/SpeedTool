using System.Numerics;
using System.Text.Json.Nodes;
using SpeedTool.Timer;

namespace SpeedTool.Splits;

public struct TimeCollection
{
    public TimeCollection()
    {

    }

    public TimeCollection(JsonObject o)
    {
        for(int i = 0; i < TIMES_COUNT; i++)
        {
            spans[i] = new TimeSpan((long)o["spans"]!.AsArray()[i]!);
        }
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

    public JsonObject ToJson()
    {
        JsonObject o = new();
        JsonArray a = new();
        for(int i = 0; i < TIMES_COUNT; i++)
        {
            a.Add((JsonNode)spans[i].Ticks);
        }

        o["spans"] = a;
        return o;
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

namespace SpeedTool.Util;

public static class TimeSpanExtension
{
    public static float FloatSeconds(this TimeSpan span)
    {
        return (span.Seconds * 1000 + span.Milliseconds) / 1000.0f;
    }

    public static float FloatMinutes(this TimeSpan span)
    {
        return (span.Minutes * 60 + span.FloatSeconds()) / 60.0f;
    }

    public static float FloatHours(this TimeSpan span)
    {
        return (span.Hours * 60 + span.FloatMinutes()) / 60.0f;
    }

    public static string ToSpeedToolTimerString(this TimeSpan span)
    {
        if(span.Hours == 0)
            return span.ToString(@"m\:ss\.fff");
        return span.ToString(@"h\:mm\:ss\.fff");
    }
}

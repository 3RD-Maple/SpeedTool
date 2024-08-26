namespace SpeedTool.Util;

public static class TimeSpanExtension
{
    /// <summary>
    /// Time format used for times less than 1 hour
    /// </summary>
    private const string TIME_FORMAT_MINUTES = @"m\:ss\.f";

    /// <summary>
    /// Time format used for times over 1 hour
    /// </summary>
    private const string TIME_FROMAT_HOURS = @"h\:mm\:ss\.f";

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
        if (span.Hours == 0)
            return span.ToString(TIME_FORMAT_MINUTES);
        return span.ToString(TIME_FROMAT_HOURS);
    }

    public static string ToSpeedtoolDTString(this TimeSpan span)
    {
        bool negative = span.Ticks < 0;
        if(negative)
            return "-" + span.ToSpeedToolTimerString();
        else
            return "+" + span.ToSpeedToolTimerString();
    }
}

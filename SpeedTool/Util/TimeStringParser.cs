namespace SpeedTool.Util;

public static class TimeStringParser
{
    public static TimeSpan ParseTimeString(string timeString)
    {
        if (string.IsNullOrWhiteSpace(timeString))
            return TimeSpan.MaxValue;

        try
        {
            timeString = timeString.Trim().Replace(" ", "");
            if (!timeString.Contains(":"))
            {
                return ParseSeconds(timeString);
            }

            string[] parts = timeString.Split(':');
            
            if(parts.Length == 2)
                return ParseMinutes(parts[0], parts[1]);
            if(parts.Length == 3)
                return ParseHours(parts[0], parts[1], parts[2]);

            return TimeSpan.MaxValue;
        }
        catch
        {
            return TimeSpan.MaxValue;
        }
    }

    public static bool IsValidTimeString(string timeString)
    {
        return ParseTimeString(timeString) != TimeSpan.MaxValue;
    }

    private static TimeSpan ParseSeconds(string timeString)
    {
        var parts = timeString.Split('.');

        int seconds = int.Parse(parts[0]);

        if(seconds >= 60 || seconds < 0)
            return TimeSpan.MaxValue;

        if (parts.Length == 1)
        {
            return TimeSpan.FromSeconds(seconds);
        }
        else if (parts.Length == 2)
        {
            int milliseconds = ParseMilliseconds(parts[1]);
            return TimeSpan.FromSeconds(seconds) + TimeSpan.FromMilliseconds(milliseconds);
        }

        return TimeSpan.MaxValue;
    }

    private static TimeSpan ParseMinutes(string minutesPart, string secondsPart)
    {
        int minutes = int.Parse(minutesPart);
        if(minutes >= 60 || minutes < 0)
            return TimeSpan.MaxValue;

        var seconds = ParseSeconds(secondsPart);

        if(seconds == TimeSpan.MaxValue)
            return TimeSpan.MaxValue;

        return TimeSpan.FromMinutes(minutes) + seconds;
    }

    private static TimeSpan ParseHours(string hoursPart, string minutesPart, string secondsPart)
    {
        int hours = int.Parse(hoursPart);
        if(hours < 0)
            return TimeSpan.MaxValue;

        var total = ParseMinutes(minutesPart, secondsPart);

        if(total == TimeSpan.MaxValue)
            return TimeSpan.MaxValue;

        return TimeSpan.FromHours(hours) + total;
    }

    private static int ParseMilliseconds(string millisString)
    {
        millisString = millisString.PadRight(3, '0').Substring(0, 3);

        return int.Parse(millisString);
    }
}

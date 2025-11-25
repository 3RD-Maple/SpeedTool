using SpeedTool.Util;

namespace SpeedTool.Tests;

public sealed class TimeStringParserTests
{
    [Fact]
    public void TestParser()
    {
        Assert.Equal(TimeSpan.FromSeconds(10), TimeStringParser.ParseTimeString("10"));
        Assert.Equal(new TimeSpan(0, 12, 13), TimeStringParser.ParseTimeString("12:13"));
        Assert.Equal(new TimeSpan(1, 45, 13), TimeStringParser.ParseTimeString("1:45:13"));
        Assert.Equal(new TimeSpan(1, 3, 13), TimeStringParser.ParseTimeString("1:03:13"));
        Assert.Equal(new TimeSpan(1, 45, 13), TimeStringParser.ParseTimeString("01:45:13"));

        Assert.Equal(new TimeSpan(0, 1, 2, 3, 457), TimeStringParser.ParseTimeString("1:02:03.457"));
        Assert.Equal(new TimeSpan(0, 1, 2, 3, 420), TimeStringParser.ParseTimeString("1:2:3.42"));
        Assert.Equal(new TimeSpan(0, 1, 2, 3, 400), TimeStringParser.ParseTimeString("1:2:3.4"));
        Assert.Equal(new TimeSpan(0, 1, 2, 3, 40), TimeStringParser.ParseTimeString("1:2:3.04"));
        Assert.Equal(new TimeSpan(0, 1, 2, 3, 4), TimeStringParser.ParseTimeString("1:2:3.004"));
        Assert.Equal(new TimeSpan(0, 1, 2, 3, 0), TimeStringParser.ParseTimeString("1:2:3.00000004"));

        Assert.Equal(new TimeSpan(0, 123, 12, 11, 987), TimeStringParser.ParseTimeString("123:12:11.987"));
    }

    [Fact]
    public void TestInvalidFormats()
    {
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString(""));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("ab:cd:ef"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:22:.123"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:13:13:13:13"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:.1333"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:13.13.13.13"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:13:13.13.13"));

        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:67:67"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("13:67:50"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("-13:13:13"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("-3:10"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("-15.133"));
        Assert.Equal(TimeSpan.MaxValue, TimeStringParser.ParseTimeString("999.133"));
    }

    [Fact]
    public void TestIsValid()
    {
        Assert.True(TimeStringParser.IsValidTimeString("11:10"));
        Assert.False(TimeStringParser.IsValidTimeString("99:99:99.999"));
    }
}

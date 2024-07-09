
using System.Diagnostics;

namespace SpeedTool.Timer;

public sealed class BasicTimer : ITimerSource
{
    public BasicTimer()
    {
        sw = new Stopwatch();
    }

    public TimeSpan CurrentTime => sw.Elapsed + accumulated;

    public TimerState CurrentState => state;

    public void Start()
    {
        sw.Start();
        state = TimerState.Running;
    }

    public void Stop()
    {
        sw.Stop();
        state = TimerState.Stopped;
    }

    public void Pause()
    {
        switch(state)
        {
        case TimerState.Paused:
            Start();
            break;
        case TimerState.Running:
            sw.Stop();
            accumulated += sw.Elapsed;
            sw.Reset();
            state = TimerState.Paused;
            break;
        default:
            break;
        }
    }

    public void Reset()
    {
        sw.Stop();
        sw.Reset();
        state = TimerState.NoState;
    }

    private TimerState state = TimerState.NoState;

    private TimeSpan accumulated;

    private Stopwatch sw;
}

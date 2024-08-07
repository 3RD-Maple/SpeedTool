
namespace SpeedTool.Timer;

/// <summary>
/// A timer that does nothing. Used for timing methods that are not in use
/// </summary>
class NoTimer : ITimerSource
{
    public TimeSpan CurrentTime => new TimeSpan(0);

    public TimerState CurrentState => TimerState.NoState;

    public void Pause() { }

    public void Reset() { }

    public void Start() { }

    public void Stop() { }
}

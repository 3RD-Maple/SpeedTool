namespace SpeedTool.Timer;

interface ITimerSource
{
    TimeSpan CurrentTime { get; }

    TimerState CurrentState { get; }

    void Pause();

    void Start();

    void Reset();

    void Stop();
}
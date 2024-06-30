namespace SpeedTool.Timer;

public enum TimerState
{
    /// <summary>
    /// A timer that has never started
    /// </summary>
    NoState,

    /// <summary>
    /// A timer that is currently running
    /// </summary>
    Running,

    /// <summary>
    /// A timer that is currently paused
    /// </summary>
    Paused,

    /// <summary>
    /// A timer that is currently stopped. A stopped timer cannot be resumed
    /// </summary>
    Stopped,

    /// <summary>
    /// A timer that is finished. A finished timer cannot be resumed
    /// </summary>
    Finished
}

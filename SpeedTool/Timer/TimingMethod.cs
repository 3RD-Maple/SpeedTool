namespace SpeedTool.Timer;

/// <summary>
/// Timing method used for time
/// </summary>
public enum TimingMethod : int
{
    /// <summary>
    /// Real-Time Attack, RTA
    /// </summary>
    RealTime = 0,

    /// <summary>
    /// Real-Time Attack minus loading screens
    /// </summary>
    Loadless,

    /// <summary>
    /// In-Game time
    /// </summary>
    InGame,

    /// <summary>
    /// Custom timing method 1
    /// </summary>
    Custom1,

    /// <summary>
    /// Custom timing method 2
    /// </summary>
    Custom2,

    /// <summary>
    /// Custom timing method 3
    /// </summary>
    Custom3,

    Last
}

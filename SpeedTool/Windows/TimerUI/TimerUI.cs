using SpeedTool.Timer;

namespace SpeedTool.Windows.TimerUI;

abstract class TimerUI
{
    public abstract void Draw(double dt, ITimerSource source);
    public abstract void DoUI(ITimerSource source);
}

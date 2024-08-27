using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.Windows.TimerUI;

abstract class TimerUIBase
{
    public abstract void Draw(double dt, ISplitsSource splits, ITimerSource source);
    public abstract void DoUI(ISplitsSource splits, ITimerSource source);


}

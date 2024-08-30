using SpeedTool.Global;
using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.Windows.TimerUI;

abstract class TimerUIBase
{
    public abstract void Draw(double dt, ISplitsSource splits, ITimerSource source);
    public abstract void DoUI(ISplitsSource splits, ITimerSource source);
    
    public virtual void ReloadConfig(object? sender, IConfigurationSection? section) { }

    protected TimingMethod DisplayTimingMethod
    {
        get
        {
            var game = Platform.Platform.SharedPlatform.Game;
            if(game == null)
                return TimingMethod.RealTime;

            return game.DefaultTimingMethod;
        }
    }
}

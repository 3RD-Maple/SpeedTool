using System.Numerics;
using Silk.NET.Windowing;
using SpeedTool.Global;
using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.Windows.TimerUI;

abstract class TimerUIBase
{
    public abstract void Draw(double dt, ISplitsSource splits, ITimerSource source);
    public abstract void DoUI(ISplitsSource splits, ITimerSource source);
    
    public virtual void ReloadConfig(object? sender, IConfigurationSection? section) { }

    public abstract WindowBorder DesiredBorder { get; }
    public virtual Vector2 DesiredSize
    {
        get
        {
            return new Vector2(500, 550);
        }
    }

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

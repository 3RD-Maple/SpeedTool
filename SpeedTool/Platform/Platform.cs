using Silk.NET.Input.Glfw;
using Silk.NET.Windowing.Glfw;
using SpeedTool.Splits;
using SpeedTool.Timer;

namespace SpeedTool.Platform;

public class Platform
{
    public static Platform SharedPlatform
    {
        get
        {
            if(platform is null)
            {
                platform = new Platform();
            }

            return platform!;
        }
    }

    public Game? Game
    { 
        get
        {
            return game;
        }
    }

    public Category? CurrentCategory
    {
        get
        {
            return game?.GetCategories()[0];
        }
    }

    public ISplitsSource GetSplits()
    {
        return run != null? run : nullSplits;
    }

    public Keyboard Keyboard => kb;

    public void Split()
    {
        if(run == null)
        {
            return;
        }

        run.Split();
    }

    public ITimerSource GetTimerFor(TimingMethod method)
    {
        return sources[(int)method];
    }

    public void LoadGame(Game game)
    {
        this.game = game;
        run = new Run(game, game.GetCategories()[0].Splits, null);
        sources[(int)TimingMethod.RealTime] = run.Timer;
    }

    public void Run()
    {
        while (windows.Count != 0)
        {
            for(int i = 0; i < windows.Count; i++)
            {
                windows[i].Cycle();
            }

            var toClose = windows.Where(x => x.IsClosed).ToList();
            windows = windows.Where(x => !x.IsClosed).ToList();
            foreach (var closed in toClose)
            {
                closed.Reset();

                // FEXME: See Platform.Window class for explanation
                //closed.Dispose();
            }
            hook.Cycle();
        }
        hook.Dispose();
    }

    public void AddWindow(Window w)
    {
        windows.Add(w);
    }

    private Platform()
    {
        GlfwWindowing.RegisterPlatform();
        GlfwInput.RegisterPlatform();

        sources = new ITimerSource[(int)TimingMethod.Last];

        sources[(int)TimingMethod.RealTime] = new BasicTimer();
        sources[(int)TimingMethod.InGame] = new NoTimer();
        sources[(int)TimingMethod.Loadless] = new NoTimer();
        sources[(int)TimingMethod.Custom1] = new NoTimer();
        sources[(int)TimingMethod.Custom2] = new NoTimer();
        sources[(int)TimingMethod.Custom3] = new NoTimer();

        kb = new Keyboard();
        windows = new List<Window>();
        hook = new KeyboardHook();
        hook.OnKeyEvent += (object? sneder, KeyPressData data) =>
        {
            kb.SetKeyData(data);
        };
    }

    public void Exit()
    {
        windows.ForEach(x => x.Close());
    }

    List<Window> windows;
    KeyboardHook hook;
    Keyboard kb;

    Run? run = null;
    Game? game = null;

    NullSplitsSource nullSplits = new();

    ITimerSource[] sources;

    private static Platform? platform;
}

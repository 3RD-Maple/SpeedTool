using System.Text.Json.Nodes;
using Silk.NET.Input.Glfw;
using Silk.NET.Windowing.Glfw;
using SpeedTool.Injector;
using SpeedTool.Platform.Debugging;
using SpeedTool.Splits;
using SpeedTool.Timer;
using SpeedTool.Util;

namespace SpeedTool.Platform;

public sealed class Platform
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

    /// <summary>
    /// Frees all plaform resources
    /// </summary>
    public void Release() // TODO: This is ugly, but I'm a bit lazy to deal with it rn
    {
        injector?.Dispose();
    }

    public Game? Game
    { 
        get
        {
            return game;
        }
    }

    public bool IsRunStarted
    {
        get
        {
            if(run == null)
                return false;
            return run.Started;
        }
    }

    public bool IsRunFinished
    {
        get
        {
            if(run == null)
                return false;
            return run.IsFinished;
        }
    }

    public Category? CurrentCategory
    {
        get
        {
            return game?.GetCategories()[activeCategory];
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
        
        if(!run.Started)
            injector?.Start();
        run.Split();
    }

    public void ResetRun()
    {
        injector?.Reset();
        ReloadRun();
    }

    public void Pause()
    {
        run?.Pause();
        injector?.Pause();
    }

    public void NextSplit()
    {
        if(run != null)
            run.SkipSplit();
    }

    public void PreviousSplit()
    {
        if(run != null)
            run.UndoSplit();
    }

    public void NextCategory()
    {
        if(game == null)
            return;
        if(run != null && run.Started)
            return;

        if(activeCategory + 1 < game!.GetCategories().Length)
            activeCategory++;

        ReloadRun();
    }

    public void PreviousCategory()
    {
        if(run != null && run.Started)
            return;
        if(activeCategory > 0)
            activeCategory--;
        
        ReloadRun();
    }

    public ITimerSource GetTimerFor(TimingMethod method)
    {
        return sources[(int)method];
    }

    public void LoadGame(Game game)
    {
        DebugLog.SharedInstance.Write($"Loading game {game.Name}");
        this.game = game;
        if(injector != null && injector.InjectedExeName != game.ExeName)
        {
            injector.Dispose();
            injector = null;
        }
        else
        {
            injector?.ReloadScript(game.Script);
        }
        ReloadRun();
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
                //closed.Reset();
                closed.Dispose();
            }
            hook.Cycle();
            hotkeyController.Cycle();
            injector?.Cycle();
        }
        hook.Dispose();
        hotkeyController.Dispose();
    }

    public void SaveRunAsPB(RunInfo run)
    {
        EnsureLocalDir();
        var fileName = game!.Name + "." + CurrentCategory!.Name;
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));
        var dst = ENV.LocalFilesPath + "pbs/" + fileName + ".json";
        File.WriteAllText(dst, run.ToJson().ToString());
    }

    public RunInfo? GetPBRun(Game g, Category c)
    {
        var fileName = game!.Name + "." + CurrentCategory!.Name;
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));

        var dst = ENV.LocalFilesPath + "pbs/" + fileName + ".json";
        if(File.Exists(dst))
            return RunInfo.FromJson(JsonNode.Parse(File.ReadAllText(dst))!.AsObject());
        return null;
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

        hotkeyController = new();
    }

    private void EnsureLocalDir()
    {
        if(!Directory.Exists(ENV.LocalFilesPath + "/pbs"))
            Directory.CreateDirectory(ENV.LocalFilesPath + "/pbs");
    }

    public void ReloadRun()
    {
        if(game == null)
            return;
        if(injector == null)
            LoadInjector();
        if(injector != null)
            injector.Reset();

        run = new Run(game, game.GetCategories()[activeCategory].Splits, GetPBRun(game, CurrentCategory!));
        sources[(int)TimingMethod.RealTime] = run.Timer;
    }

    public void Exit()
    {
        windows.ForEach(x => x.Close());
    }

    private void LoadInjector()
    {
        if(game == null)
            return;

        if(InjectorHandler.IsInjectionAvailable)
        {
            injector?.Dispose();
            injector = game.ExeName == "" ? null : new InjectorHandler(game.ExeName);
            sources[(int)TimingMethod.Loadless] = injector == null ? new NoTimer() : injector;
        }
        else
            DebugLog.SharedInstance.Write("Injector unavailable, skipping injection");
    }

    List<Window> windows;
    KeyboardHook hook;
    Keyboard kb;

    Run? run = null;
    Game? game = null;

    InjectorHandler? injector;

    HotkeyController hotkeyController;

    NullSplitsSource nullSplits = new();

    ITimerSource[] sources;

    int activeCategory = 0;

    private static Platform? platform;
}

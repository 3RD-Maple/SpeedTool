using ImGuiNET;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using SpeedTool.Timer;
using SpeedTool.Windows.Drawables;
using SpeedTool.Windows.TimerUI;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;
using SpeedTool.Splits;

namespace SpeedTool.Windows;

using SPWindow = Platform.Window;

class MainWindow : SPWindow, IDisposable
{
    public MainWindow() : base(options, new Vector2D<int>(500, 550))
    {
        platform = Platform.Platform.SharedPlatform;
        drw = new TimerDrawable(Gl);
        config = Configuration.GetSection<GeneralConfiguration>()!;
        ui = SelectUI();
    }

    protected override void OnConfigUpdated(object? sender, IConfigurationSection? section)
    {
        if (!(section is GeneralConfiguration))
        {
            return;
        }
        
        // event handling 
    }

    override public void Dispose()
    {
        drw.Dispose();
        base.Dispose();
        Configuration.OnConfigurationChanged -= OnConfigUpdated;
    }

    protected override void OnDraw(double dt)
    {
        ui.Draw(dt, platform.GetSplits(), platform.GetTimerFor(TimingMethod.RealTime));
        if(platform.Game != null)
            Text = $"Speedtool -- {platform.Game.Name} -- {platform.CurrentCategory?.Name}";
        else
            Text = "Speedtool";
    }

    protected override void OnAfterUI(double dt)
    {
    }

    protected override void OnLoad()
    {
        LoadFontEx(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\segoeui.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\meiryo.ttc", 42, "Main");
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.PushFont(GetFont("Main"));
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ui.DoUI(platform.GetSplits(), platform.GetTimerFor(TimingMethod.RealTime));

        // Opening windows in between ImGui calls screws up ImGui's stack, so we need to do that on function exit
        Action? onExit = null;

        ImGui.PopFont();
        ImGui.PushFont(GetFont("UI"));

        if(ImGui.BeginPopupContextWindow())
        {
            if(ImGui.MenuItem("Edit Game"))
            {
                onExit = () => platform.AddWindow(platform.Game == null ? new GameEditorWindow() :
                                                                          new GameEditorWindow(platform.Game));
            }
            if(platform.Game != null)
            {
                if(ImGui.MenuItem("Next Category"))
                {
                    platform.NextCategory();
                }
                if(ImGui.MenuItem("Previous Category"))
                {
                    platform.PreviousCategory();
                }
            }
            ImGui.Separator();
            if(ImGui.MenuItem("Split"))
            {
                platform.Split();
            }
            if(ImGui.MenuItem("Pause"))
            {
                platform.Pause();
            }
            if(ImGui.MenuItem("Reset run"))
            {
                platform.ResetRun();
            }
            ImGui.Separator();
            if(ImGui.BeginMenu("UI Select"))
            {
                if(ImGui.MenuItem("SpeedTool"))
                {
                    config.TimerUI = "SpeedTool";
                    Configuration.SetSection(config);
                    ui = SelectUI();
                }
                if(ImGui.MenuItem("Classic"))
                {
                    config.TimerUI = "Classic";
                    Configuration.SetSection(config);
                    ui = SelectUI();
                }
                ImGui.EndMenu();
            }
            if(ImGui.MenuItem("Settings"))
            {
                onExit = () => platform.AddWindow(new SettingsWindow());
            }
            if(ImGui.MenuItem("About"))
            {
                onExit = () => platform.AddWindow(new AboutWindow());
            }
            if(ImGui.MenuItem("Exit"))
            {
                platform.Exit();
            }
            ImGui.EndPopup();
        }

        if(hasError)
        {
            hasError = false;
            ImGui.OpenPopup("Error");
        }

        bool open = true;
        if(ImGui.BeginPopupModal("Error", ref open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text(errorMessage);
            if(ImGui.Button("OK"))
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
        ImGui.PopFont();
        ImGui.End();

        onExit?.Invoke();
    }

    protected override void OnFilesDropped(string[] files)
    {
        if(files.Length != 1) // Only one file is allowed
            return;

        try
        {
            platform.LoadGame(Game.LoadFromFile(files[0]));
        }
        catch(Exception)
        {
            hasError = true;
            errorMessage = "Broken file";
        }
    }

    static private WindowOptions options
    {
        get
        {
            var opts = WindowOptions.Default;
            opts.Samples = 8;
            opts.WindowBorder = WindowBorder.Fixed;
            return opts;
        }
    }

    private TimerUIBase SelectUI()
    {
        switch(Configuration.GetSection<GeneralConfiguration>()!.TimerUI)
        {
        case "Classic":
            return new ClassicTimerUI();
        case "SpeedTool":
            return new SpeedToolTimerUI(Gl);
        default:
            return new SpeedToolTimerUI(Gl);
        }
    }

    bool hasError = false;
    string errorMessage = "";
    TimerDrawable drw;
    private GeneralConfiguration config;
    Platform.Platform platform;

    TimerUIBase ui;
}

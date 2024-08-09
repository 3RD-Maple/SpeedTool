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
        timer = new BasicTimer();

        ui = SelectUI();
    }

    override public void Dispose()
    {
        drw.Dispose();
        base.Dispose();
    }

    protected override void OnDraw(double dt)
    {
        ui.Draw(dt, timer);
        if(platform.Game != null)
            Text = $"Speedtool -- {platform.Game.Name}";
        else
            Text = "Speedtool";
    }

    protected override void OnAfterUI(double dt)
    {
    }

    protected override void OnLoad()
    {
        Platform.Platform.SharedPlatform.LoadFont("C:\\Windows\\Fonts\\cour.ttf", 42);
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.PushFont(platform.GetFont(0));
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ui.DoUI(timer);

        // Opening windows in between ImGui calls screws up ImGui's stack, so we need to do that on function exit
        Action? onExit = null;

        ImGui.PopFont();

        if(ImGui.BeginPopupContextWindow())
        {
            if(ImGui.MenuItem("Edit Game"))
            {
                onExit = () => platform.AddWindow(platform.Game == null ? new GameEditorWindow() :
                                                                          new GameEditorWindow(platform.Game));
            }
            ImGui.Separator();
            if(ImGui.MenuItem("Split"))
            {
                platform.Split();
            }
            if(ImGui.MenuItem("Pause"))
            {
                // TODO: Add controls
            }
            ImGui.Separator();
            if(ImGui.BeginMenu("UI Select"))
            {
                if(ImGui.MenuItem("SpeedTool"))
                {
                    var conf = Configuration.GetSection<GeneralConfiguration>()!;
                    conf.TimerUI = "SpeedTool";
                    Configuration.SetSection(conf);
                    ui = SelectUI();
                }
                if(ImGui.MenuItem("Classic"))
                {
                    var conf = Configuration.GetSection<GeneralConfiguration>()!;
                    conf.TimerUI = "Classic";
                    Configuration.SetSection(conf);
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

    BasicTimer timer;
    TimerDrawable drw;

    Platform.Platform platform;

    TimerUIBase ui;
}

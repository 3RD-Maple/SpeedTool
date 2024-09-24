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
using System.Runtime.InteropServices;
using SpeedTool.Platform.Linux;
using System.Diagnostics.CodeAnalysis;

namespace SpeedTool.Windows;

using SPWindow = Platform.Window;

class MainWindow : SPWindow
{
    public MainWindow() : base(options, new Vector2D<int>(500, 550))
    {
        platform = Platform.Platform.SharedPlatform;
        drw = new TimerDrawable(Gl);
        config = Configuration.GetSection<GeneralConfiguration>();
        colorSettings = Configuration.GetSection<ColorSettings>();
        BackgroundColor = colorSettings.TimerBackground;
        LoadUI();
    }

    protected override void OnConfigUpdated(object? sender, IConfigurationSection? section)
    {
        ui.ReloadConfig(sender, section);
        if ((section as GeneralConfiguration) != null)
        {
            config = (section as GeneralConfiguration)!;
        }
        else if((section as ColorSettings) != null)
        {
            colorSettings = (section as ColorSettings)!;
            BackgroundColor = colorSettings.TimerBackground;
        }
    }

    override public void Dispose()
    {
        drw.Dispose();
        base.Dispose();
        Configuration.OnConfigurationChanged -= OnConfigUpdated;
    }

    protected override void OnDraw(double dt)
    {
        var tm = platform.Game == null ? TimingMethod.RealTime : platform.Game.DefaultTimingMethod;
        ui.Draw(dt, platform.GetSplits(), platform.GetTimerFor(tm));
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
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            LoadFontEx(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\segoeui.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\meiryo.ttc", 42, "Main");
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            LoadFontEx(Fonts.DefaultFont, Fonts.DefaultCJKFont, 42, "Main");
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.PushFont(GetFont("Main"));
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        var tm = platform.Game == null ? TimingMethod.RealTime : platform.Game.DefaultTimingMethod;
        ui.DoUI(platform.GetSplits(), platform.GetTimerFor(tm));

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
                    SetUI("SpeedTool");
                }
                if(ImGui.MenuItem("Classic"))
                {
                    SetUI("Classic");
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
            if(ImGui.MenuItem("Debug log"))
            {
                onExit = () => platform.AddWindow(new DebugMessagesWindow());
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
        switch(Configuration.GetSection<GeneralConfiguration>().TimerUI)
        {
        case "Classic":
            return new ClassicTimerUI();
        case "SpeedTool":
            return new SpeedToolTimerUI(Gl);
        default:
            return new SpeedToolTimerUI(Gl);
        }
    }

    private void SetUI(string display)
    {
        if(config.TimerUI == display)
            return;

        config.TimerUI = display;
        Configuration.SetSection(config);
        LoadUI();
    }

    [MemberNotNull(nameof(ui))]
    private void LoadUI()
    {
        ui = SelectUI();
        SetBorder(ui.DesiredBorder);
        Resize(new Vector2D<int>((int)ui.DesiredSize.X, (int)ui.DesiredSize.Y));
    }

    bool hasError = false;
    string errorMessage = "";
    TimerDrawable drw;
    private GeneralConfiguration config;
    private ColorSettings colorSettings;
    Platform.Platform platform;

    TimerUIBase ui;
}

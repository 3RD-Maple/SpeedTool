using ImGuiNET;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using SpeedTool.Timer;
using SpeedTool.Windows.Drawables;
using SpeedTool.Windows.TimerUI;

namespace SpeedTool.Windows;

using SPWindow = Platform.Window;

class MainWindow : SPWindow, IDisposable
{
    public MainWindow() : base(options, new Vector2D<int>(500, 550))
    {
        platform = Platform.Platform.SharedPlatform;
        drw = new TimerDrawable(Gl);
        timer = new BasicTimer();

        ui = new ClassicTimerUI();
    }

    override public void Dispose()
    {
        drw.Dispose();
        base.Dispose();
    }

    protected override void OnDraw(double dt)
    {
        ui.Draw(dt, timer);
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

        // TODO: This is test stuff, will be removed later
        ImGui.Text("Last pressed: ");
        ImGui.SameLine();
        ImGui.Text(platform.Keyboard.LastPressed.ToString());

        ImGui.PopFont();

        if(ImGui.BeginPopupContextWindow())
        {
            if(ImGui.MenuItem("Edit Game"))
            {
                onExit = () => platform.AddWindow(new GameEditorWindow());
            }
            ImGui.Separator();
            if(ImGui.MenuItem("Split"))
            {
                // TODO: Add controls
            }
            if(ImGui.MenuItem("Pause"))
            {
                // TODO: Add controls
            }
            ImGui.Separator();
            if(ImGui.MenuItem("Settings"))
            {
                onExit = () => platform.AddWindow(new SettingsWindow());
            }
            if(ImGui.MenuItem("Exit"))
            {
                platform.Exit();
            }
            ImGui.EndPopup();
        }

        ImGui.End();

        onExit?.Invoke();
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

    BasicTimer timer;
    TimerDrawable drw;

    Platform.Platform platform;

    TimerUIBase ui;
}

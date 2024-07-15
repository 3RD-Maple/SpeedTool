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
        base.Dispose();
        drw.Dispose();
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
        bool t = false;
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.PushFont(platform.GetFont(0));
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ui.DoUI(timer);

        ImGui.End();
        ImGui.PopFont();
        if(t)
            platform.AddWindow(new SPWindow(WindowOptions.Default, new Vector2D<int>(500, 500)));
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

using ImGuiNET;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using SpeedTool.Timer;
using SpeedTool.Windows.Drawables;

namespace SpeedTool.Windows;

using SPWindow = Platform.Window;

class MainWindow : SPWindow, IDisposable
{
    public MainWindow() : base(options, new Vector2D<int>(500, 550))
    {
        platform = Platform.Platform.SharedPlatform;
        drw = new TimerDrawable(Gl);
        timer = new BasicTimer();
    }

    override public void Dispose()
    {
        base.Dispose();
        drw.Dispose();
    }

    protected override void OnDraw(double dt)
    {
        Gl.Viewport(0, 0, 500, 500);
        drw.Draw(timer);
    }

    protected override void OnAfterUI(double dt)
    {
    }

    protected override void OnLoad()
    {
        Platform.Platform.SharedPlatform.LoadFont("C:\\Windows\\Fonts\\calibri.ttf", 42);
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
        if(ImGui.Button("Hello"))
            timer.Start();
        ImGui.SameLine();
        if(ImGui.Button("Other"))
            timer.Pause();
        
        ImGui.Text(counter.ToString());
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

    private int counter = 0;
}

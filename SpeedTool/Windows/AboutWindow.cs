using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Util.ImGui;

namespace SpeedTool.Windows;

class AboutWindow : Platform.Window
{
    public AboutWindow() : base(options, new Vector2D<int>(500, 300))
    {
        platform = Platform.Platform.SharedPlatform;
        Text = "About SpeedTool";
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);

        ImGuiExtensions.TextCentered($"Speedtool ver {version}");
        ImGuiExtensions.TextCentered("© 3RD MAPLE");
        ImGuiExtensions.TextCentered("CC BY-SA 4.0");
        ImGui.Text(" ");
        ImGui.Text("By:");
        ImGui.Text("    Alexey Drozhzhin aka Grim Maple, the Lead Programmer");
        ImGui.Text("    Dmitry \"HDFox\" Khakrizoyev, the C# Programmer");
        ImGui.Text("    Alexander \"Rusty Skull\" Bogdanov, the C# Consultant");
        ImGui.Text("    Anastasiia Drozhzhina aka POPUGAICHIK, the Designer");
        ImGui.Text(" ");
        ImGui.Text("    And all SpeedTool Contributors!");

        ImGui.End();
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

    Platform.Platform platform;
}

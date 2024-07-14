using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Window = SpeedTool.Platform.Window;

namespace SpeedTool.Windows;

public sealed class SettingsWindow : Window
{
    private Platform.Platform platform = Platform.Platform.SharedPlatform;

    private Vector3 textColor = new(0.3f, 0.7f, 0.9f);

    public SettingsWindow() : base(options, new Vector2D<int>(500, 550))
    {
    }

    private static WindowOptions options
    {
        get
        {
            var opts = WindowOptions.Default;
            opts.Samples = 8;
            opts.WindowBorder = WindowBorder.Fixed;
            return opts;
        }
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        //ImGui.PushFont(platform.GetFont(0)); //to be fixed
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("SettingsWindow",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoMove);


        ImGui.TextColored(new Vector4(textColor, 1.0f), "Text color");
        ImGui.SameLine();


        if (ImGui.ColorButton("Text color", new Vector4(textColor, 1.0f), ImGuiColorEditFlags.None,
                new Vector2(25f, 25f))) ImGui.OpenPopup("TextColorPicker");

        if (ImGui.BeginPopup("TextColorPicker"))
        {
            ImGui.ColorPicker3("Choose text color", ref textColor);

            ImGui.EndPopup();
        }

        ImGui.End();
        //ImGui.PopFont();
    }
}
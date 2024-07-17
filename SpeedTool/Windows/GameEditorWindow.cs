using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace SpeedTool.Windows;

public sealed class GameEditorWindow : Platform.Window
{
    private readonly Platform.Platform platform;
    public GameEditorWindow() : base(WindowOptions.Default, new Vector2D<int>(800, 600))
    {
        platform = Platform.Platform.SharedPlatform;
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("MainWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        ImGui.InputText("##name", ref Name, 255);
        ImGui.SameLine();
        ImGui.Text("Game Name");

        ImGui.End();
    }

    private string Name = "";
}

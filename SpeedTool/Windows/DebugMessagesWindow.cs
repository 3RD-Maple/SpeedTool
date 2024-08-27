using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SpeedTool.Platform.Debugging;

namespace SpeedTool.Windows;

public sealed class DebugMessagesWindow : Platform.Window
{
    public DebugMessagesWindow() : base(WindowOptions.Default, new Vector2D<int>(800, 600))
    {
    }

    protected override void OnUI(double dt)
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.PushFont(GetFont("UI"));
        ImGui.SetNextWindowBgAlpha(0.0f);
        ImGui.Begin("DebugMessagesWindowWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove);

        var messages = DebugLog.SharedInstance.GetMessages(500);
        foreach(var msg in messages)
        {
            ImGui.Text(msg);
            ImGui.Separator();
        }

        ImGui.End();
    }
}

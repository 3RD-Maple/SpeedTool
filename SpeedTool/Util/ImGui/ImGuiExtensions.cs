using System.Numerics;

namespace SpeedTool.Util.ImGui;

public static class ImGuiExtensions
{
    public static void SpeedToolColorPicker(string text, ref Vector4 color)
    {
        SpeedToolColorPicker(text, ref color, new Vector4(1));
    }
    
    public static void SpeedToolColorPicker(string text, ref Vector4 color, Vector4 textColor)
    {
        if (ImGuiNET.ImGui.ColorButton($"{text}", color, ImGuiNET.ImGuiColorEditFlags.None, new Vector2(25f, 25f)))
        {
            ImGuiNET.ImGui.OpenPopup($"{text}ColorPicker");
        }

        ImGuiNET.ImGui.SameLine();
        ImGuiNET.ImGui.TextColored(textColor, text);
        
        if (ImGuiNET.ImGui.BeginPopup($"{text}ColorPicker"))
        {
            ImGuiNET.ImGui.ColorPicker4($"Choose {text.ToLower()}", ref color);
            ImGuiNET.ImGui.EndPopup();
        }
    }
}
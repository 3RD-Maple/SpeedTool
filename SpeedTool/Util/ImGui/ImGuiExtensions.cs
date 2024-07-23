using System.Numerics;
using SharpHook.Native;
using SpeedTool.Platform;

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

    public static void SpeedToolHotkey(string name, ref Hotkey hotkey)
    {
        var isIgnoredKey = (KeyCode x) => ignoredKeyCodes.Any(z => x == z);

        var kb = Platform.Platform.SharedPlatform.Keyboard;
        string text = hotkey.DisplayString;
        ImGuiNET.ImGui.InputText(name, ref text, 255, ImGuiNET.ImGuiInputTextFlags.ReadOnly);
        if(ImGuiNET.ImGui.IsItemActive())
        {
            if(!listening)
            {
                lastPressed = kb.LastPressed;
                listening = true;
            }
            if(lastPressed != kb.LastPressed && !isIgnoredKey(kb.LastPressed))
            {
                hotkey.Alt = kb.IsPresed(KeyCode.VcLeftAlt) || kb.IsPresed(KeyCode.VcRightAlt);
                hotkey.Ctrl = kb.IsPresed(KeyCode.VcLeftControl) || kb.IsPresed(KeyCode.VcRightControl);
                hotkey.Shift = kb.IsPresed(KeyCode.VcLeftShift) || kb.IsPresed(KeyCode.VcRightShift);
                hotkey.Key = kb.LastPressed;
                lastPressed = kb.LastPressed;
                listening = false;
                ResetFocus();
            }
        }
    }

    /// <summary>
    ///  Remove focus from any given element
    /// </summary>
    public static void ResetFocus()
    {
        // HACK: Creates an invisible label on the same line and forces focus there,
        //       Therefore removing focus from any other element on the screen
        ImGuiNET.ImGui.SameLine();
        ImGuiNET.ImGui.Text("");
        ImGuiNET.ImGui.SetKeyboardFocusHere();
    }

    private static KeyCode lastPressed = KeyCode.VcUndefined;
    private static bool listening = false;

    private static readonly KeyCode[] ignoredKeyCodes = { KeyCode.VcLeftAlt, KeyCode.VcRightAlt,
                                                          KeyCode.VcLeftControl, KeyCode.VcRightControl,
                                                          KeyCode.VcLeftShift, KeyCode.VcRightShift };
}
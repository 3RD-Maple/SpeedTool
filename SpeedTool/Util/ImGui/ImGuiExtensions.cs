using System.Numerics;
using SharpHook.Native;
using SpeedTool.Platform;
using SpeedTool.Splits;

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
                hotkey.Alt = kb.IsPressed(KeyCode.VcLeftAlt) || kb.IsPressed(KeyCode.VcRightAlt);
                hotkey.Ctrl = kb.IsPressed(KeyCode.VcLeftControl) || kb.IsPressed(KeyCode.VcRightControl);
                hotkey.Shift = kb.IsPressed(KeyCode.VcLeftShift) || kb.IsPressed(KeyCode.VcRightShift);
                hotkey.Key = kb.LastPressed;
                lastPressed = kb.LastPressed;
                listening = false;
                ResetFocus();
            }
        }
    }

    public static SpeedToolSplitContext SpeedToolSplit(string name, ref Split split)
    {
        SpeedToolSplitContext ctx = new();
        if(split.Subsplits.Length == 0)
        {
            ImGuiNET.ImGui.InputText(name, ref split.Name, 255);
            ctx.IsHovered = ImGuiNET.ImGui.IsItemHovered();
            ctx.selectedSplit = ctx.IsHovered ? split : null;
            return ctx;
        }

        bool isOpen = ImGuiNET.ImGui.TreeNodeEx(name + "Tree", ImGuiNET.ImGuiTreeNodeFlags.AllowOverlap);
        ImGuiNET.ImGui.SameLine();
        ImGuiNET.ImGui.InputText(name, ref split.Name, 255);
        ctx.IsHovered = ImGuiNET.ImGui.IsItemHovered();
        ctx.selectedSplit = ctx.IsHovered ? split : null;
        if(isOpen)
        {
            for(int i = 0; i < split.Subsplits.Length; i++)
            {
                var res = SpeedToolSplit("##" + name + "subsplit" + i.ToString(), ref split.Subsplits[i]);
                if(res.selectedSplit != null)
                    ctx.selectedSplit = res.selectedSplit;
                ctx.parentSplit = split;
            }
            ImGuiNET.ImGui.TreePop();
        }

        return ctx;
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
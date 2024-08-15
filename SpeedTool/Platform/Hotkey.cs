using System.Text.Json.Nodes;

namespace SpeedTool.Platform;

using System.Text;
using SharpHook.Native;

public struct Hotkey
{
    public Hotkey() { }
    public bool Alt = false;
    public bool Ctrl = false;
    public bool Shift = false;

    public KeyCode Key = KeyCode.VcSpace;

    public bool IsTriggered
    {
        get
        {
            var kb = Platform.SharedPlatform.Keyboard;
            var controls = AreCorrectControlsPressed();
            return kb.IsPressed(Key) && controls;
        }
    }

    public string DisplayString
    {
        get
        {
            StringBuilder sb = new();
            if(Alt)
                sb.Append("ALT + ");
            if(Ctrl)
                sb.Append("CTR + ");
            if(Shift)
                sb.Append("SHIFT + ");
            sb.Append(Key.ToString());
            return sb.ToString();
        }
    }
    
    public static JsonObject ToJSONObject(Hotkey hotkey)
    { 
        var node = new JsonObject();

        node["CheckAlt"] = hotkey.Alt;
        node["CheckShift"] = hotkey.Shift;
        node["CheckCtrl"] = hotkey.Ctrl;
        node["KeyCode"] = (ushort)hotkey.Key;
        
        return node;
    }
    
    public static Hotkey FromJSONObject(JsonObject node) // use instead of class constructor 
    {
        var hotkey = new Hotkey();
        
        hotkey.Alt = (bool)node["CheckAlt"]!;
        hotkey.Shift = (bool)node["CheckShift"]!;
        hotkey.Ctrl = (bool)node["CheckCtrl"]!;
        
        hotkey.Key =  (KeyCode)((ushort)node["KeyCode"]!);

        return hotkey;
    }

    private bool AreCorrectControlsPressed()
    {
        var kb = Platform.SharedPlatform.Keyboard;
        var isAltPressed = kb.IsPressed(KeyCode.VcLeftAlt) || kb.IsPressed(KeyCode.VcRightAlt);
        var isCtrlPressed = kb.IsPressed(KeyCode.VcLeftControl) || kb.IsPressed(KeyCode.VcRightControl);
        var isShiftPressed = kb.IsPressed(KeyCode.VcLeftShift) || kb.IsPressed(KeyCode.VcRightShift);

        var altCheckPassed = Alt ? isAltPressed : !isAltPressed;
        var ctrlCheckPassed = Ctrl ? isCtrlPressed : !isCtrlPressed;
        var shiftCheckPassed = Shift ? isShiftPressed : !isShiftPressed;

        return altCheckPassed && ctrlCheckPassed && shiftCheckPassed;
    }
}

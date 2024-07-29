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

    bool IsTriggered
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

        node["IsAltPressed"] = hotkey.Alt;
        node["IsShiftPressed"] = hotkey.Shift;
        node["IsCtrlPressed"] = hotkey.Ctrl;
        node["KeyCode"] = (ushort)hotkey.Key;
        
        return node;
    }
    
    public static Hotkey FromJSONObject(JsonObject node) // use instead of class constructor 
    {
        var hotkey = new Hotkey();
        
        hotkey.Alt = (bool)node["IsAltPressed"]!;
        hotkey.Shift = (bool)node["IsShiftPressed"]!;
        hotkey.Ctrl = (bool)node["IsCtrlPressed"]!;
        
        hotkey.Key =  (KeyCode)((ushort)node["KeyCode"]!);

        return hotkey;
    }

    private bool AreCorrectControlsPressed()
    {
        var kb = Platform.SharedPlatform.Keyboard;
        var isAltPressed = kb.IsPressed(KeyCode.VcLeftAlt) || kb.IsPressed(KeyCode.VcRightAlt);
        var isCtrlPressed = kb.IsPressed(KeyCode.VcLeftControl) || kb.IsPressed(KeyCode.VcRightControl);
        var isShiftPressed = kb.IsPressed(KeyCode.VcLeftShift) || kb.IsPressed(KeyCode.VcRightShift);

        var altCheckPassed = Alt ? isAltPressed : true;
        var ctrlCheckPassed = Ctrl ? isCtrlPressed : true;
        var shiftCheckPassed = Shift ? isShiftPressed : true;

        return altCheckPassed && ctrlCheckPassed && shiftCheckPassed;
    }
}

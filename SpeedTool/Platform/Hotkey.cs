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
            return kb.IsPresed(Key) && controls;
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

    private bool AreCorrectControlsPressed()
    {
        var kb = Platform.SharedPlatform.Keyboard;
        var isAltPressed = kb.IsPresed(KeyCode.VcLeftAlt) || kb.IsPresed(KeyCode.VcRightAlt);
        var isCtrlPressed = kb.IsPresed(KeyCode.VcLeftControl) || kb.IsPresed(KeyCode.VcRightControl);
        var isShiftPressed = kb.IsPresed(KeyCode.VcLeftShift) || kb.IsPresed(KeyCode.VcRightShift);

        var altCheckPassed = Alt ? isAltPressed : true;
        var ctrlCheckPassed = Ctrl ? isCtrlPressed : true;
        var shiftCheckPassed = Shift ? isShiftPressed : true;

        return altCheckPassed && ctrlCheckPassed && shiftCheckPassed;
    }
}

using SharpHook.Native;

namespace SpeedTool.Platform;

public struct KeyPressData
{
    public KeyPressData(bool pressed, KeyCode keyCode)
    {
        Pressed = pressed;
        KeyCode = keyCode;
    }

    /// <summary>
    /// Was the key pressed or depressed
    /// </summary>
    public bool Pressed { get; private set; }

    public KeyCode KeyCode { get; private set;}
}

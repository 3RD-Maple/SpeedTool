using System.Runtime.CompilerServices;
using SharpHook.Native;

namespace SpeedTool.Platform;

public sealed class Keyboard
{
    public KeyCode LastPressed { get; private set; }
    public KeyCode LastReleased { get; private set; }

    internal Keyboard()
    {

    }

    public bool IsPresed(KeyCode keyCode)
    {
        var key = (int)keyCode;
        return key > 255 ? false : keys[key];
    }

    internal void SetKeyData(KeyPressData data)
    {
        /// TODO: For now only check for 255 keys, they are the default keyboard keys
        if((int)data.KeyCode > 255)
            return;
        keys[(int)data.KeyCode] = data.Pressed;
        if(data.Pressed)
            LastPressed = data.KeyCode;
        else
            LastReleased = data.KeyCode;
    }

    private bool[] keys = new bool[256];
}

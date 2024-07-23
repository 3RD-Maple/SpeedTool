using System.Runtime.InteropServices;
using SharpHook.Native;

namespace SpeedTool.Platform;

public sealed class Keyboard
{
    public KeyCode LastPressed { get; private set; }
    public KeyCode LastReleased { get; private set; }

    internal Keyboard() { }

    public bool IsPresed(KeyCode keyCode)
    {
        var key = (ushort)keyCode;
        return keys[key];
    }

    internal void SetKeyData(KeyPressData data)
    {
        keys[(ushort)data.KeyCode] = data.Pressed;

        if(data.Pressed)
            LastPressed = data.KeyCode;
        else
            LastReleased = data.KeyCode;
    }

    // 65 kbytes of madness. At least it should be 65 kbytes
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 3)]
    private bool[] keys = new bool[ushort.MaxValue];
}

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SharpHook.Native;

namespace SpeedTool.Platform.Linux;

/// <summary>
/// Keyboard event source reading evdev devices directly.
/// Global hooks rely on X11, which is unavailable under Wayland, so this backend
/// is used there instead. It requires read access to /dev/input/event* devices,
/// which usually means being a member of the "input" group.
/// </summary>
internal sealed class EvdevKeyboardHook : IKeyboardEventSource
{
    public event EventHandler<KeyPressData>? OnKeyEvent;

    public bool IsAvailable => devices.Count != 0;

    public EvdevKeyboardHook()
    {
        running = true;
        ScanDevices();
        worker = new Thread(WorkerLoop) { IsBackground = true, Name = "EvdevKeyboardHook" };
        worker.Start();
    }

    public void Dispose()
    {
        running = false;
        worker?.Join();
        foreach(var stream in devices.Values)
            stream.Dispose();
        devices.Clear();
    }

    private void WorkerLoop()
    {
        var waited = 0;
        while(running)
        {
            Thread.Sleep(RescanCheckIntervalMs);
            waited += RescanCheckIntervalMs;
            if(!running)
                break;

            if(waited >= RescanIntervalMs)
            {
                waited = 0;
                ScanDevices();
            }
        }
    }

    private void ScanDevices()
    {
        if(!Directory.Exists(InputDirectory))
            return;

        foreach(var path in Directory.GetFiles(InputDirectory, "event*").Order())
        {
            if(devices.ContainsKey(path) || !IsKeyboard(path))
                continue;

            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if(!devices.TryAdd(path, stream))
                {
                    stream.Dispose();
                    continue;
                }

                var thread = new Thread(() => ReadLoop(path, stream)) { IsBackground = true, Name = "EvdevKeyboardReader" };
                thread.Start();
            }
            catch(Exception)
            {
                // No permission to read this device
            }
        }
    }

    private void ReadLoop(string path, FileStream stream)
    {
        var buffer = new byte[Marshal.SizeOf<InputEvent>()];

        while(running)
        {
            int read;
            try
            {
                read = stream.ReadAtLeast(buffer, buffer.Length, false);
            }
            catch(Exception)
            {
                break;
            }

            if(read < buffer.Length)
                break;

            var ev = MemoryMarshal.Read<InputEvent>(buffer);
            if(ev.Type != EV_KEY || ev.Value == KeyValueRepeated)
                continue;

            if(!KeyMap.TryGetValue(ev.Code, out var key))
                continue;

            OnKeyEvent?.Invoke(this, new KeyPressData(ev.Value == KeyValuePressed, key));
        }

        devices.TryRemove(path, out _);
        stream.Dispose();
    }

    private static bool IsKeyboard(string device)
    {
        try
        {
            var name = Path.GetFileName(device);
            var capabilities = File.ReadAllText($"/sys/class/input/{name}/device/capabilities/key");
            return HasCapabilityBit(capabilities, (int)EvdevKey.KEY_ESC) && HasCapabilityBit(capabilities, (int)EvdevKey.KEY_SPACE);
        }
        catch
        {
            // Can't tell, assume keyboard and let the event filter do its job
            return true;
        }
    }

    private static bool HasCapabilityBit(string capabilities, int bit)
    {
        var words = capabilities.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var word = Convert.ToUInt64(words[words.Length - 1 - bit / 64], 16);
        return (word & (1ul << (bit % 64))) != 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct InputEvent
    {
        public nuint TimeSec;
        public nuint TimeUsec;
        public ushort Type;
        public ushort Code;
        public int Value;
    }

    private static readonly Dictionary<ushort, KeyCode> KeyMap = new()
    {
        [(ushort)EvdevKey.KEY_ESC] = KeyCode.VcEscape,
        [(ushort)EvdevKey.KEY_1] = KeyCode.Vc1,
        [(ushort)EvdevKey.KEY_2] = KeyCode.Vc2,
        [(ushort)EvdevKey.KEY_3] = KeyCode.Vc3,
        [(ushort)EvdevKey.KEY_4] = KeyCode.Vc4,
        [(ushort)EvdevKey.KEY_5] = KeyCode.Vc5,
        [(ushort)EvdevKey.KEY_6] = KeyCode.Vc6,
        [(ushort)EvdevKey.KEY_7] = KeyCode.Vc7,
        [(ushort)EvdevKey.KEY_8] = KeyCode.Vc8,
        [(ushort)EvdevKey.KEY_9] = KeyCode.Vc9,
        [(ushort)EvdevKey.KEY_0] = KeyCode.Vc0,
        [(ushort)EvdevKey.KEY_MINUS] = KeyCode.VcMinus,
        [(ushort)EvdevKey.KEY_EQUAL] = KeyCode.VcEquals,
        [(ushort)EvdevKey.KEY_BACKSPACE] = KeyCode.VcBackspace,
        [(ushort)EvdevKey.KEY_TAB] = KeyCode.VcTab,
        [(ushort)EvdevKey.KEY_Q] = KeyCode.VcQ,
        [(ushort)EvdevKey.KEY_W] = KeyCode.VcW,
        [(ushort)EvdevKey.KEY_E] = KeyCode.VcE,
        [(ushort)EvdevKey.KEY_R] = KeyCode.VcR,
        [(ushort)EvdevKey.KEY_T] = KeyCode.VcT,
        [(ushort)EvdevKey.KEY_Y] = KeyCode.VcY,
        [(ushort)EvdevKey.KEY_U] = KeyCode.VcU,
        [(ushort)EvdevKey.KEY_I] = KeyCode.VcI,
        [(ushort)EvdevKey.KEY_O] = KeyCode.VcO,
        [(ushort)EvdevKey.KEY_P] = KeyCode.VcP,
        [(ushort)EvdevKey.KEY_LEFTBRACE] = KeyCode.VcOpenBracket,
        [(ushort)EvdevKey.KEY_RIGHTBRACE] = KeyCode.VcCloseBracket,
        [(ushort)EvdevKey.KEY_ENTER] = KeyCode.VcEnter,
        [(ushort)EvdevKey.KEY_LEFTCTRL] = KeyCode.VcLeftControl,
        [(ushort)EvdevKey.KEY_A] = KeyCode.VcA,
        [(ushort)EvdevKey.KEY_S] = KeyCode.VcS,
        [(ushort)EvdevKey.KEY_D] = KeyCode.VcD,
        [(ushort)EvdevKey.KEY_F] = KeyCode.VcF,
        [(ushort)EvdevKey.KEY_G] = KeyCode.VcG,
        [(ushort)EvdevKey.KEY_H] = KeyCode.VcH,
        [(ushort)EvdevKey.KEY_J] = KeyCode.VcJ,
        [(ushort)EvdevKey.KEY_K] = KeyCode.VcK,
        [(ushort)EvdevKey.KEY_L] = KeyCode.VcL,
        [(ushort)EvdevKey.KEY_SEMICOLON] = KeyCode.VcSemicolon,
        [(ushort)EvdevKey.KEY_APOSTROPHE] = KeyCode.VcQuote,
        [(ushort)EvdevKey.KEY_GRAVE] = KeyCode.VcBackQuote,
        [(ushort)EvdevKey.KEY_LEFTSHIFT] = KeyCode.VcLeftShift,
        [(ushort)EvdevKey.KEY_BACKSLASH] = KeyCode.VcBackslash,
        [(ushort)EvdevKey.KEY_Z] = KeyCode.VcZ,
        [(ushort)EvdevKey.KEY_X] = KeyCode.VcX,
        [(ushort)EvdevKey.KEY_C] = KeyCode.VcC,
        [(ushort)EvdevKey.KEY_V] = KeyCode.VcV,
        [(ushort)EvdevKey.KEY_B] = KeyCode.VcB,
        [(ushort)EvdevKey.KEY_N] = KeyCode.VcN,
        [(ushort)EvdevKey.KEY_M] = KeyCode.VcM,
        [(ushort)EvdevKey.KEY_COMMA] = KeyCode.VcComma,
        [(ushort)EvdevKey.KEY_DOT] = KeyCode.VcPeriod,
        [(ushort)EvdevKey.KEY_SLASH] = KeyCode.VcSlash,
        [(ushort)EvdevKey.KEY_RIGHTSHIFT] = KeyCode.VcRightShift,
        [(ushort)EvdevKey.KEY_KPASTERISK] = KeyCode.VcNumPadMultiply,
        [(ushort)EvdevKey.KEY_LEFTALT] = KeyCode.VcLeftAlt,
        [(ushort)EvdevKey.KEY_SPACE] = KeyCode.VcSpace,
        [(ushort)EvdevKey.KEY_CAPSLOCK] = KeyCode.VcCapsLock,
        [(ushort)EvdevKey.KEY_F1] = KeyCode.VcF1,
        [(ushort)EvdevKey.KEY_F2] = KeyCode.VcF2,
        [(ushort)EvdevKey.KEY_F3] = KeyCode.VcF3,
        [(ushort)EvdevKey.KEY_F4] = KeyCode.VcF4,
        [(ushort)EvdevKey.KEY_F5] = KeyCode.VcF5,
        [(ushort)EvdevKey.KEY_F6] = KeyCode.VcF6,
        [(ushort)EvdevKey.KEY_F7] = KeyCode.VcF7,
        [(ushort)EvdevKey.KEY_F8] = KeyCode.VcF8,
        [(ushort)EvdevKey.KEY_F9] = KeyCode.VcF9,
        [(ushort)EvdevKey.KEY_F10] = KeyCode.VcF10,
        [(ushort)EvdevKey.KEY_NUMLOCK] = KeyCode.VcNumLock,
        [(ushort)EvdevKey.KEY_SCROLLLOCK] = KeyCode.VcScrollLock,
        [(ushort)EvdevKey.KEY_KP7] = KeyCode.VcNumPad7,
        [(ushort)EvdevKey.KEY_KP8] = KeyCode.VcNumPad8,
        [(ushort)EvdevKey.KEY_KP9] = KeyCode.VcNumPad9,
        [(ushort)EvdevKey.KEY_KPMINUS] = KeyCode.VcNumPadSubtract,
        [(ushort)EvdevKey.KEY_KP4] = KeyCode.VcNumPad4,
        [(ushort)EvdevKey.KEY_KP5] = KeyCode.VcNumPad5,
        [(ushort)EvdevKey.KEY_KP6] = KeyCode.VcNumPad6,
        [(ushort)EvdevKey.KEY_KPPLUS] = KeyCode.VcNumPadAdd,
        [(ushort)EvdevKey.KEY_KP1] = KeyCode.VcNumPad1,
        [(ushort)EvdevKey.KEY_KP2] = KeyCode.VcNumPad2,
        [(ushort)EvdevKey.KEY_KP3] = KeyCode.VcNumPad3,
        [(ushort)EvdevKey.KEY_KP0] = KeyCode.VcNumPad0,
        [(ushort)EvdevKey.KEY_KPDOT] = KeyCode.VcNumPadDecimal,
        [(ushort)EvdevKey.KEY_F11] = KeyCode.VcF11,
        [(ushort)EvdevKey.KEY_F12] = KeyCode.VcF12,
        [(ushort)EvdevKey.KEY_KPENTER] = KeyCode.VcNumPadEnter,
        [(ushort)EvdevKey.KEY_RIGHTCTRL] = KeyCode.VcRightControl,
        [(ushort)EvdevKey.KEY_KPSLASH] = KeyCode.VcNumPadDivide,
        [(ushort)EvdevKey.KEY_RIGHTALT] = KeyCode.VcRightAlt,
        [(ushort)EvdevKey.KEY_HOME] = KeyCode.VcHome,
        [(ushort)EvdevKey.KEY_UP] = KeyCode.VcUp,
        [(ushort)EvdevKey.KEY_PAGEUP] = KeyCode.VcPageUp,
        [(ushort)EvdevKey.KEY_LEFT] = KeyCode.VcLeft,
        [(ushort)EvdevKey.KEY_RIGHT] = KeyCode.VcRight,
        [(ushort)EvdevKey.KEY_END] = KeyCode.VcEnd,
        [(ushort)EvdevKey.KEY_DOWN] = KeyCode.VcDown,
        [(ushort)EvdevKey.KEY_PAGEDOWN] = KeyCode.VcPageDown,
        [(ushort)EvdevKey.KEY_INSERT] = KeyCode.VcInsert,
        [(ushort)EvdevKey.KEY_DELETE] = KeyCode.VcDelete,
        [(ushort)EvdevKey.KEY_MUTE] = KeyCode.VcVolumeMute,
        [(ushort)EvdevKey.KEY_VOLUMEUP] = KeyCode.VcVolumeUp,
        [(ushort)EvdevKey.KEY_VOLUMEDOWN] = KeyCode.VcVolumeDown,
        [(ushort)EvdevKey.KEY_KPEQUAL] = KeyCode.VcNumPadEquals,
        [(ushort)EvdevKey.KEY_PAUSE] = KeyCode.VcPause,
        [(ushort)EvdevKey.KEY_LEFTMETA] = KeyCode.VcLeftMeta,
        [(ushort)EvdevKey.KEY_RIGHTMETA] = KeyCode.VcRightMeta,
        [(ushort)EvdevKey.KEY_COMPOSE] = KeyCode.VcContextMenu,
        [(ushort)EvdevKey.KEY_NEXTSONG] = KeyCode.VcMediaNext,
        [(ushort)EvdevKey.KEY_PLAYPAUSE] = KeyCode.VcMediaPlay,
        [(ushort)EvdevKey.KEY_PREVIOUSSONG] = KeyCode.VcMediaPrevious,
        [(ushort)EvdevKey.KEY_STOPCD] = KeyCode.VcMediaStop,
    };

    private enum EvdevKey : ushort
    {
        KEY_ESC = 1,
        KEY_1 = 2,
        KEY_2 = 3,
        KEY_3 = 4,
        KEY_4 = 5,
        KEY_5 = 6,
        KEY_6 = 7,
        KEY_7 = 8,
        KEY_8 = 9,
        KEY_9 = 10,
        KEY_0 = 11,
        KEY_MINUS = 12,
        KEY_EQUAL = 13,
        KEY_BACKSPACE = 14,
        KEY_TAB = 15,
        KEY_Q = 16,
        KEY_W = 17,
        KEY_E = 18,
        KEY_R = 19,
        KEY_T = 20,
        KEY_Y = 21,
        KEY_U = 22,
        KEY_I = 23,
        KEY_O = 24,
        KEY_P = 25,
        KEY_LEFTBRACE = 26,
        KEY_RIGHTBRACE = 27,
        KEY_ENTER = 28,
        KEY_LEFTCTRL = 29,
        KEY_A = 30,
        KEY_S = 31,
        KEY_D = 32,
        KEY_F = 33,
        KEY_G = 34,
        KEY_H = 35,
        KEY_J = 36,
        KEY_K = 37,
        KEY_L = 38,
        KEY_SEMICOLON = 39,
        KEY_APOSTROPHE = 40,
        KEY_GRAVE = 41,
        KEY_LEFTSHIFT = 42,
        KEY_BACKSLASH = 43,
        KEY_Z = 44,
        KEY_X = 45,
        KEY_C = 46,
        KEY_V = 47,
        KEY_B = 48,
        KEY_N = 49,
        KEY_M = 50,
        KEY_COMMA = 51,
        KEY_DOT = 52,
        KEY_SLASH = 53,
        KEY_RIGHTSHIFT = 54,
        KEY_KPASTERISK = 55,
        KEY_LEFTALT = 56,
        KEY_SPACE = 57,
        KEY_CAPSLOCK = 58,
        KEY_F1 = 59,
        KEY_F2 = 60,
        KEY_F3 = 61,
        KEY_F4 = 62,
        KEY_F5 = 63,
        KEY_F6 = 64,
        KEY_F7 = 65,
        KEY_F8 = 66,
        KEY_F9 = 67,
        KEY_F10 = 68,
        KEY_NUMLOCK = 69,
        KEY_SCROLLLOCK = 70,
        KEY_KP7 = 71,
        KEY_KP8 = 72,
        KEY_KP9 = 73,
        KEY_KPMINUS = 74,
        KEY_KP4 = 75,
        KEY_KP5 = 76,
        KEY_KP6 = 77,
        KEY_KPPLUS = 78,
        KEY_KP1 = 79,
        KEY_KP2 = 80,
        KEY_KP3 = 81,
        KEY_KP0 = 82,
        KEY_KPDOT = 83,
        KEY_F11 = 87,
        KEY_F12 = 88,
        KEY_KPENTER = 96,
        KEY_RIGHTCTRL = 97,
        KEY_KPSLASH = 98,
        KEY_RIGHTALT = 100,
        KEY_HOME = 102,
        KEY_UP = 103,
        KEY_PAGEUP = 104,
        KEY_LEFT = 105,
        KEY_RIGHT = 106,
        KEY_END = 107,
        KEY_DOWN = 108,
        KEY_PAGEDOWN = 109,
        KEY_INSERT = 110,
        KEY_DELETE = 111,
        KEY_MUTE = 113,
        KEY_VOLUMEDOWN = 114,
        KEY_VOLUMEUP = 115,
        KEY_KPEQUAL = 117,
        KEY_PAUSE = 119,
        KEY_LEFTMETA = 125,
        KEY_RIGHTMETA = 126,
        KEY_COMPOSE = 127,
        KEY_NEXTSONG = 163,
        KEY_PLAYPAUSE = 164,
        KEY_PREVIOUSSONG = 165,
        KEY_STOPCD = 166,
    }

    private const string InputDirectory = "/dev/input";
    private const ushort EV_KEY = 0x01;
    private const int KeyValuePressed = 1;
    private const int KeyValueRepeated = 2;
    private const int RescanIntervalMs = 5000;
    private const int RescanCheckIntervalMs = 500;

    private Thread worker;
    private volatile bool running;

    ConcurrentDictionary<string, FileStream> devices = new();
}

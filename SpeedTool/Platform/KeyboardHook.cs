using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SpeedTool.Platform.Debugging;
using SpeedTool.Platform.Linux;
using SharpHook;

namespace SpeedTool.Platform;

/// <summary>
/// Global keyboard event source shared by all hotkeys.
/// Chooses the backend automatically: a global X11 hook where it is available,
/// and direct evdev reading on Wayland sessions, where global hooks cannot work.
/// The backend can be forced with the SPEEDTOOL_HOTKEY_BACKEND environment variable
/// ("evdev" or "global").
/// </summary>
internal sealed class KeyboardHook : IDisposable
{
    /// <summary>
    /// Dispatched when key event is raised (key pressed, key released)
    /// </summary>
    public event EventHandler<KeyPressData>? OnKeyEvent;

    public KeyboardHook()
    {
        impl = CreateHook();
        impl.OnKeyEvent += (object? sender, KeyPressData data) =>
        {
            presses.Enqueue(data);
        };
    }

    public void Cycle()
    {
        while(!presses.IsEmpty)
        {
            var keyData = DequeueElement();
            OnKeyEvent?.Invoke(this, keyData);
        }
    }

    public void Dispose()
    {
        impl.Dispose();
    }

    private IKeyboardEventSource CreateHook()
    {
        if(!UseEvdevBackend())
        {
            DebugLog.SharedInstance.Write("Keyboard hook: using global hook");
            return new SharpHookKeyboardHook();
        }

        var evdev = new EvdevKeyboardHook();
        if(evdev.IsAvailable)
        {
            DebugLog.SharedInstance.Write("Keyboard hook: using evdev backend (Wayland session)");
            return evdev;
        }

        evdev.Dispose();
        DebugLog.SharedInstance.Write("Keyboard hook: no evdev access, falling back to global hook. " +
                                      "To enable global hotkeys on Wayland add yourself to the 'input' group: sudo usermod -aG input $USER");
        return new SharpHookKeyboardHook();
    }

    private static bool UseEvdevBackend()
    {
        if(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return false;

        switch(Environment.GetEnvironmentVariable(BackendEnvironmentVariable)?.ToLowerInvariant())
        {
        case "evdev":
            return true;
        case "global":
        case "x11":
            return false;
        }

        return IsWaylandSession();
    }

    private static bool IsWaylandSession()
    {
        return Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") != null ||
               Environment.GetEnvironmentVariable("XDG_SESSION_TYPE") == "wayland";
    }

    private KeyPressData DequeueElement()
    {
        KeyPressData data;
        while(!presses.TryDequeue(out data));
        return data;
    }

    private const string BackendEnvironmentVariable = "SPEEDTOOL_HOTKEY_BACKEND";

    private IKeyboardEventSource impl;

    ConcurrentQueue<KeyPressData> presses = new();
}

internal interface IKeyboardEventSource : IDisposable
{
    /// <summary>
    /// Dispatched when key event is raised (key pressed, key released)
    /// </summary>
    event EventHandler<KeyPressData>? OnKeyEvent;
}

internal sealed class SharpHookKeyboardHook : IKeyboardEventSource
{
    public event EventHandler<KeyPressData>? OnKeyEvent;

    public SharpHookKeyboardHook()
    {
        hook = new SimpleGlobalHook();
        hook.KeyPressed += Hook_KeyPressed;
        hook.KeyReleased += Hook_KeyReleased;
        hookTask = hook.RunAsync();
    }

    public void Dispose()
    {
        hook.Dispose();
        try
        {
            hookTask.Wait();
        }
        catch(Exception)
        {
            // The hook task faults when there is no display to attach to, nothing to do about it
        }
    }

    private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs args)
    {
        OnKeyEvent?.Invoke(this, new KeyPressData(true, args.Data.KeyCode));
    }

    private void Hook_KeyReleased(object? sender, KeyboardHookEventArgs args)
    {
        OnKeyEvent?.Invoke(this, new KeyPressData(false, args.Data.KeyCode));
    }

    private Task hookTask;
    private SimpleGlobalHook hook;
}

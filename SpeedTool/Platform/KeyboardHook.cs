using System.Collections.Concurrent;
using SharpHook;

namespace SpeedTool.Platform;

internal sealed class KeyboardHook : IDisposable
{
    /// <summary>
    /// Dispatched when key event is raised (key pressed, key released)
    /// </summary>
    public event EventHandler<KeyPressData>? OnKeyEvent;

    public KeyboardHook()
    {
        #if DEBUG
        #else
        hook = new SimpleGlobalHook();
        hook.KeyPressed += Hook_KeyPressed;
        hook.KeyReleased += Hook_KeyReleased;
        hookTask = hook.RunAsync();
        #endif
    }

    public void Cycle()
    {
        #if DEBUG
        #else
        while(!presses.IsEmpty)
        {
            var keyData = DequeueElement();
            OnKeyEvent?.Invoke(this, keyData);
        }
        #endif
    }

    public void Dispose()
    {
        #if DEBUG
        #else
        hook.Dispose();
        hookTask.Wait();
        #endif
    }

    private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs args)
    {
        OnKeyEvent?.Invoke(null, new KeyPressData());
        presses.Enqueue(new KeyPressData(true, args.Data.KeyCode));
    }

    private void Hook_KeyReleased(object? sender, KeyboardHookEventArgs args)
    {
        presses.Enqueue(new KeyPressData(false, args.Data.KeyCode));
    }

    private KeyPressData DequeueElement()
    {
        KeyPressData data;
        while(!presses.TryDequeue(out data));
        return data;
    }
    #if DEBUG
    #else
    private Task hookTask;
    private SimpleGlobalHook hook;
    #endif

    ConcurrentQueue<KeyPressData> presses = new();
}

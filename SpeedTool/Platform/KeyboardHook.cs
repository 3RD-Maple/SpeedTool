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
        hook = new SimpleGlobalHook();
        hook.KeyPressed += Hook_KeyPressed;
        hook.KeyReleased += Hook_KeyReleased;
        hookTask = hook.RunAsync();
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
        hook.Dispose();
        hookTask.Wait();
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
    private Task hookTask;
    private SimpleGlobalHook hook;

    ConcurrentQueue<KeyPressData> presses = new();
}

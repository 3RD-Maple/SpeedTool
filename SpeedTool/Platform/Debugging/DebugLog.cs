using System.Collections.Concurrent;

namespace SpeedTool.Platform.Debugging;

public sealed class DebugLog
{
    public static DebugLog SharedInstance
    {
        get
        {
            if(instance != null)
            {
                return instance!;
            }
            lock(lockObj)
            {
                instance = new DebugLog();
            }

            return instance!;
        }
    }

    public void Write(string message)
    {
        messages.Enqueue(message);

        if(messages.Count > 1000)
        {
            string? tmp = "";
            while(messages.Count > 500) messages.TryDequeue(out tmp);
        }
    }

    public IEnumerable<string> GetMessages(int count)
    {
        return messages.TakeLast(count);
    }

    private ConcurrentQueue<string> messages = new();

    private static object lockObj = new();

    private static DebugLog? instance;
}

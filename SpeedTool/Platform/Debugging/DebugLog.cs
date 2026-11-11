using System.Collections.Concurrent;
using SpeedTool.Util;

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

        Console.WriteLine(message);
        WriteToFile(message);
    }

    public IEnumerable<string> GetMessages(int count)
    {
        return messages.TakeLast(count);
    }

    private void WriteToFile(string message)
    {
        try
        {
            Directory.CreateDirectory(ENV.LocalFilesPath);
            File.AppendAllText(Path.Combine(ENV.LocalFilesPath, "debug.log"), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}\n");
        }
        catch
        {
            // Nowhere to write the log, keep it in memory only
        }
    }

    private ConcurrentQueue<string> messages = new();

    private static object lockObj = new();

    private static DebugLog? instance;
}

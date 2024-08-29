using System.Diagnostics;
using SpeedTool.Platform.Debugging;

namespace SpeedTool.Injector;

public sealed class InjectorHandler : IDisposable
{
    public InjectorHandler(string lookForExe)
    {
        exeName = lookForExe.Replace(".exe", "");
        workerThread = new Thread(Worker);
        workerThread.Start();
        DebugLog.SharedInstance.Write($"Attempting to inject timer into {lookForExe}");
    }

    public static bool IsInjectionAvailable
    {
        get
        {
            return File.Exists("TimerInjector.exe");
        }
    }

    public void Cycle()
    {
        if(!IsHooked || p == null)
            return;

        p.Cycle();
    }

    public bool IsHooked { get; private set; }

    private void Worker()
    {
        while(!IsClosing)
        {
            if(IsHooked)
            {
                Thread.Sleep(10);
                if(p != null)
                {
                    if(!p.IsOpened)
                    {
                        IsHooked = false;
                        DebugLog.SharedInstance.Write("Pipe connection lost!");
                        DebugLog.SharedInstance.Write("Attempting to re-inject");
                        p.Dispose();
                        p = null;
                    }
                }
                continue;
            }
            if(!IsHooked)
            {
                var process = Process.GetProcessesByName(exeName);
                if(process.Length > 0)
                {
                    loadedProcess = process[0];
                    IsHooked = true;
                    DebugLog.SharedInstance.Write($"Found {exeName}, injecting");
                }
                else
                {
                    Thread.Sleep(10);
                    continue;
                }
            }
            var psi = new ProcessStartInfo()
            {
                FileName = "TimerInjector.exe",
                Arguments = loadedProcess!.Id.ToString(),
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            var injector = Process.Start(psi);
            if(injector == null)
            {
                DebugLog.SharedInstance.Write($"Failed to start injector process");
                IsHooked = false;
                continue;
            }
            injector.WaitForExit();
            if(injector.ExitCode != 0)
            {
                DebugLog.SharedInstance.Write($"Failed to inject timer with code {injector.ExitCode}");
                IsHooked = false;
            }
            else
            {
                DebugLog.SharedInstance.Write($"Injecting successful, connecting to pipe now");
                p = new Pipe();
            }
        }
    }

    public void Dispose()
    {
        IsClosing = true;
        workerThread.Join();
        p?.Dispose();
    }

    bool IsClosing = false;
    Pipe? p;

    Thread workerThread;
    Process? loadedProcess;
    private string exeName;
}

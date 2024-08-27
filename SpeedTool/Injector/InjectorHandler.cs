using System.Diagnostics;
using SpeedTool.Platform.Debugging;

namespace SpeedTool.Injector;

public sealed class InjectorHandler
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

    public bool IsHooked { get; private set; }

    private void Worker()
    {
        while(true)
        {
            if(IsHooked)
            {
                Thread.Sleep(1000);
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
                    Thread.Sleep(1000);
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
                DebugLog.SharedInstance.Write($"Injecting successful");
        }
    }

    Thread workerThread;
    Process? loadedProcess;
    private string exeName;
}

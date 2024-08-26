using System.Diagnostics;

namespace SpeedTool.Injector;

public sealed class InjectorHandler
{
    public InjectorHandler(string lookForExe)
    {
        exeName = lookForExe.Replace(".exe", "");
        workerThread = new Thread(Worker);
        workerThread.Start();
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
                IsHooked = false;
                continue;
            }
            injector.WaitForExit();
            if(injector.ExitCode != 0)
            {
                IsHooked = false;
            }
        }
    }

    Thread workerThread;
    Process? loadedProcess;
    private string exeName;
}

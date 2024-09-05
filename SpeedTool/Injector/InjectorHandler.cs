using System.Diagnostics;
using SpeedTool.Platform.Debugging;
using SpeedTool.Timer;

namespace SpeedTool.Injector;

public sealed class InjectorHandler : IDisposable, ITimerSource
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

    public TimeSpan CurrentTime => time;

    public TimerState CurrentState => TimerState.NoState;

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
                        p.OnMessage -= OnMessage;
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
                p = new Pipe(Platform.Platform.SharedPlatform.Game!.Script);
                p.OnMessage += OnMessage;
            }
        }
    }

    public void Dispose()
    {
        IsClosing = true;
        workerThread.Join();
        p?.Dispose();
    }

    public void Pause()
    {
        p?.SendString("pause");
    }

    public void Start()
    {
        p?.SendString("start");
    }

    public void Reset()
    {
        p?.SendString("reset");
    }

    public void Stop()
    {
        p?.SendString("stop");
    }

    private void OnMessage(object? sender, string message)
    {
        if(message.StartsWith("timer "))
        {
            var ticks = long.Parse(message.Split(" ")[1]);
            time = new TimeSpan(ticks);
            return;
        }
        if(message == "start")
        {
            if(!Platform.Platform.SharedPlatform.IsRunStarted)
                Platform.Platform.SharedPlatform.Split();
        }
        if(message == "split")
        {
            Platform.Platform.SharedPlatform.Split();
        }
    }

    TimeSpan time;

    bool IsClosing = false;
    Pipe? p;

    Thread workerThread;
    Process? loadedProcess;
    private string exeName;
}

using System.Diagnostics;
using System.Runtime.InteropServices;
using SpeedTool.Platform.Debugging;
using SpeedTool.Timer;

namespace SpeedTool.Injector;

public sealed class InjectorHandler : IDisposable, ITimerSource
{
    public InjectorHandler(string lookForExe)
    {
        exeName = lookForExe.Replace(".exe", "");
        InjectedExeName = lookForExe;
        DebugLog.SharedInstance.Write($"Attempting to inject timer into {lookForExe}");
        if(File.Exists(@"\\.\pipe\SpeedToolPipe"))
        {
            DebugLog.SharedInstance.Write($"Already injected, reconnecting");
            IsHooked = true;
            ConnectToPipe();
        }
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

    public void Cycle()
    {
        if(!IsHooked || p == null)
            return;

        p.Cycle();
    }

    public bool IsHooked { get; private set; }

    public TimeSpan CurrentTime => time;

    public TimerState CurrentState
    {
        get
        {
            if(time.Ticks == 0)
                return TimerState.NoState;
            return TimerState.Running;
        }
    }

    public void ReloadScript(string script)
    {
        p?.SendScript(script);
    }

    public string InjectedExeName { get; private set; }

    private void Worker()
    {
        while(!IsClosing)
        {
            if(IsHooked)
            {
                Thread.Sleep(50);
                if(p != null)
                {
                    if(!p.IsOpened)
                    {
                        IsHooked = false;
                        DebugLog.SharedInstance.Write("Pipe connection lost!");
                        DebugLog.SharedInstance.Write("Attempting to re-inject");
                        DisconnectPipe();
                        Thread.Sleep(500);
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
                    PrepareLUALibrary();
                }
                else
                {
                    Thread.Sleep(500);
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
                ConnectToPipe();
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
        time = new(0);
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

    private void PrepareLUALibrary()
    {
        var exeDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        var luaLib = exeDir + "/lua54.dll";
        if(File.Exists(luaLib))
            File.Delete(luaLib);

        if(IsWin64Emulator())
        {
            File.Copy(exeDir + "/x86/lua54.dll", luaLib);
        }
        else
        {
            File.Copy(exeDir + "/x64/lua54.dll", luaLib);
        }
    }

    private bool IsWin64Emulator()
    {
        if(loadedProcess == null)
            return false;
        if ((Environment.OSVersion.Version.Major > 5)
            || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
        {
            bool retVal;

            return IsWow64Process(loadedProcess.Handle, out retVal) && retVal;
        }

        return false; // not on 64-bit Windows Emulator
    }

    private void ConnectToPipe()
    {
        DisconnectPipe();
        p = new Pipe(Platform.Platform.SharedPlatform.Game!.Script);
        p.OnMessage += OnMessage;
    }

    private void DisconnectPipe()
    {
        if(p != null)
        {
            p.OnMessage -= OnMessage;
            p.Dispose();
            p = null;
        }
    }

    TimeSpan time;

    bool IsClosing = false;
    Pipe? p;

    Thread workerThread;
    Process? loadedProcess;
    private string exeName;

    [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
}

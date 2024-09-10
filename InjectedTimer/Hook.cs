using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EasyHook;
using InjectedTimer;
using InjectedTimer.Hooking;

namespace Hook
{
    public class InjectionEntryPoint: IEntryPoint
    {
        /// <summary>
        /// Message queue of all files accessed
        /// </summary>
        Queue<string> _messageQueue = new Queue<string>();

        /// <summary>
        /// EasyHook requires a constructor that matches <paramref name="context"/> and any additional parameters as provided
        /// in the original call to <see cref="EasyHook.RemoteHooking.Inject(int, EasyHook.InjectionOptions, string, string, object[])"/>.
        /// 
        /// Multiple constructors can exist on the same <see cref="EasyHook.IEntryPoint"/>, providing that each one has a corresponding Run method (e.g. <see cref="Run(EasyHook.RemoteHooking.IContext, string)"/>).
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public InjectionEntryPoint(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {
        }

        /// <summary>
        /// The main entry point for our logic once injected within the target process. 
        /// This is where the hooks will be created, and a loop will be entered until host process exits.
        /// EasyHook requires a matching Run method for the constructor
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public void Run(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {
            // Install hooks

            // Wake up the process (required if using RemoteHooking.CreateAndInject)
            EasyHook.RemoteHooking.WakeUpProcess();
            shutdownToken = new CancellationTokenSource();
            var server = Task.Run(() => CreateServer(shutdownToken.Token), shutdownToken.Token);

            while(p == null)
                Thread.Sleep(1);

            try
            {
                hook = PresentationHookBase.AutoDetectCreateHook();
                p.SendString("debug_message Game Auto-detected as " + hook.HookName);
                hook.OnFrame = CycleInternal;
            }
            catch
            {
                p.SendString("debug_message Failed to auto-detect; Scripting is disabled");
            }
            p.Cycle();

            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(500);

                    string[] queued = null;

                    lock (_messageQueue)
                    {
                        queued = _messageQueue.ToArray();
                        _messageQueue.Clear();
                    }
                }
            }
            catch
            {
            }

            shutdownToken.Cancel();
            server.Wait();

            // Remove hooks
            hook.Dispose();

            // Finalise cleanup of hooks
            EasyHook.LocalHook.Release();
        }

        void OnMsg(object sender, string cmd)
        {
            if(cmd.StartsWith("script "))
            {
                script += cmd.Substring(7) + "\n";
                return;
            }
            if(cmd.StartsWith("script_load"))
            {
                p.SendString("debug_message Script loaded");
                if(engine != null)
                    engine.Dispose();
                engine = new ScriptEngine(script, p);
                script = "";
                return;
            }
            if(cmd.StartsWith("start"))
            {
                timer = new InjectedTimer.Timer();
                timer.Start();
                p.SendString("debug_message Timer starting");
                return;
            }
            if(cmd == "reset")
            {
                timer = null;
                p.SendString("debug_message Timer reset");
                return;
            }
        }

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint SwapBuffers(IntPtr unnamedParam1);

        private void CycleInternal()
        {
            try
            {
                if(p != null)
                    p.Cycle();
                if(engine != null)
                    engine.OnFrame();
                if(timer != null)
                {
                    timer.Cycle(ScriptFunction.IsLoading);
                    if(p != null)
                        p.SendString("timer " + timer.Value.Ticks.ToString());
                }
            }
            catch(Exception ex)
            {
                if(p != null)
                    p.SendString("debug_message " + ex.Message.Replace("/r", "").Replace("\n", "__"));
            }
        }

        async void CreateServer(CancellationToken can)
        {
            while(!can.IsCancellationRequested)
            {
                if(p != null)
                {
                    p.OnIncomingCmd -= OnMsg;
                    p.Dispose();
                }
                p = new Pipe();
                p.OnIncomingCmd += OnMsg;
                while(p.IsOk) await Task.Delay(500);
            }
        }

        string script;
        CancellationTokenSource shutdownToken;
        Pipe p;
        ScriptEngine engine;
        InjectedTimer.Timer timer;
        PresentationHookBase hook;
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;
using InjectedTimer;

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

            bool d3d9 = false;
            p = new Pipe();
            LocalHook buffersHook = null;
            if(APIHelper.HasModule("d3d9.dll"))
            {
                d3d9 = true;
                p.SendString("debug_message Detected as Direct3D 9");
                while(dxHook == null)
                {
                    try
                    {
                        dxHook = new DirectXHook
                        {
                            OnFrame = () => CycleInternal()
                        };
                    }
                    catch
                    {
                        p.SendString("debug_message Installing DirectX hook failed");
                        p.Cycle();
                        Thread.Sleep(1000);
                    }
                }
            }

            if(!d3d9)
                buffersHook = LocalHook.Create(LocalHook.GetProcAddress("Gdi32.dll", "SwapBuffers"), new SwapBuffers_Delegate(MySwapBuffers), this);
            if(buffersHook != null)
                buffersHook.ThreadACL.SetExclusiveACL( new int[] { 0 });

            p.OnIncomingCmd += (object sender, string cmd) =>
            {
                if(cmd.StartsWith("script "))
                {
                    script += cmd.Substring(7) + "\n";
                    return;
                }
                if(cmd.StartsWith("script_load"))
                {
                    p.SendString("debug_message Script loaded");
                    engine = new ScriptEngine(script, p);
                    return;
                }
                if(cmd.StartsWith("start"))
                {
                    timer = new InjectedTimer.Timer();
                    timer.Start();
                    p.SendString("debug_message Timer starting");
                    return;
                }
            };

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

            // Remove hooks
            buffersHook.Dispose();

            // Finalise cleanup of hooks
            EasyHook.LocalHook.Release();
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

        uint MySwapBuffers(IntPtr unnamedParam1)
        {
            CycleInternal();
            return SwapBuffers(unnamedParam1);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
        delegate uint SwapBuffers_Delegate(IntPtr unnamedParam1);

        string script;

        Pipe p;
        ScriptEngine engine;
        InjectedTimer.Timer timer;
        DirectXHook dxHook;
    }
}

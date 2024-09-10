using System;
using System.Runtime.InteropServices;
using EasyHook;

namespace InjectedTimer.Hooking
{
    public sealed class SwapBuffersHook : PresentationHookBase
    {
        internal SwapBuffersHook()
        {
            swapBuffers = new HookedFunction<SwapBuffers_Delegate>(LocalHook.GetProcAddress("Gdi32.dll", "SwapBuffers"), MySwapBuffers);
        }

        uint MySwapBuffers(IntPtr unnamedParam1)
        {
            OnFrame?.Invoke();
            return swapBuffers.Original(unnamedParam1);
        }

        protected override void Release()
        {
            swapBuffers.Dispose();
            base.Release();
        }

        public override string HookName => "OpenGL (SwapBuffers)";

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate uint SwapBuffers_Delegate(IntPtr unnamedParam1);

        HookedFunction<SwapBuffers_Delegate> swapBuffers;
    }
}

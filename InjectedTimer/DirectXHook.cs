using System;
using System.Runtime.InteropServices;
using InjectedTimer.D3D;

namespace InjectedTimer
{
    public sealed class DirectXHook
    {
        public static unsafe IntPtr GetHookPtr()
        {
            return IntPtr.Zero;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using EasyHook;

namespace InjectedTimer.Hooking
{
    public sealed class HookedFunction<T> : IDisposable where T : Delegate
    {
        public HookedFunction(IntPtr dst, T newFunc)
        {
            original = Marshal.GetDelegateForFunctionPointer<T>(dst);
            destination = dst;
            hook = LocalHook.Create(dst, newFunc, this);
            hook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
        }

        public T Original => original;
        LocalHook hook;
        T original;
        IntPtr destination;

        public void Dispose()
        {
            hook.Dispose();
        }
    }
}

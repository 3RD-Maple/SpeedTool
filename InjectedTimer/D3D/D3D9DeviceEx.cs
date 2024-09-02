using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    public sealed class D3D9DeviceEx : IDisposable
    {
        internal unsafe D3D9DeviceEx(IntPtr ptr)
        {
            if(IntPtr.Zero == ptr)
            {
                throw new Exception("Device pointer cannot be zero");
            }

            this.ptr = ptr;
            vtbl = *(IntPtr*)ptr;
        }

        public unsafe IntPtr PresentExFuncPtr
        {
            get
            {
                return ((IntPtr*)vtbl)[121];
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int PresentEx_Delegate(IntPtr pThis, IntPtr pSourceRect, IntPtr pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion, int dwFlags);

        public void Dispose()
        {
            Marshal.Release(ptr);
        }

        IntPtr ptr;
        IntPtr vtbl;
    }
}

using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    public sealed class D3D9Device : IDisposable
    {
        internal unsafe D3D9Device(IntPtr ptr)
        {
            if(IntPtr.Zero == ptr)
            {
                throw new Exception("Device pointer cannot be zero");
            }

            this.ptr = ptr;
            vtbl = *(IntPtr*)ptr;
        }

        public unsafe IntPtr EndSceneFuncPtr
        {
            get
            {
                return ((IntPtr*)vtbl)[42];
            }
        }

        public unsafe IntPtr PresentFuncPtr
        {
            get
            {
                return ((IntPtr*)vtbl)[17];
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int EndScene_Delegate(IntPtr pThis);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int Present_Delegate(IntPtr pThis, IntPtr pSourceRect, IntPtr pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion);

        public void Dispose()
        {
            Marshal.Release(ptr);
        }

        IntPtr ptr;
        IntPtr vtbl;
    }
}

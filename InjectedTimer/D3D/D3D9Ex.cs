using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    public class D3D9Ex : IDisposable
    {
        public unsafe D3D9Ex()
        {
            Direct3DCreate9Ex(D3DHook.SDK_VERSION, out d3d);
            vtbl = *(IntPtr*)d3d;
            NativeCreateDeviceEx = Marshal.GetDelegateForFunctionPointer<CreateDeviceExPtr>(((IntPtr*)vtbl)[20]);
        }

        public D3D9DeviceEx CreateDeviceEx(uint Adapter, int DeviceType, IntPtr hWnd, int flags, D3DHook.D3DPRESENT_PARAMETERS PresentationParams)
        {
            IntPtr result = IntPtr.Zero;
            int hresult = NativeCreateDeviceEx(d3d, Adapter, DeviceType, hWnd, flags, ref PresentationParams, IntPtr.Zero, out result);
            if(result == IntPtr.Zero)
            {
                throw new Exception("Cannot create D3DEx Device");
            }

            return new D3D9DeviceEx(result);
        }

        public void Dispose()
        {
            if(d3d != IntPtr.Zero)
                Marshal.Release(d3d);
        }

        IntPtr d3d;
        IntPtr vtbl;

        private CreateDeviceExPtr NativeCreateDeviceEx;

        delegate int CreateDeviceExPtr(IntPtr pThis, uint Adapter, int DeviceType, IntPtr hwnd, int flags, [In] ref D3DHook.D3DPRESENT_PARAMETERS pPresentationParams, IntPtr MustBeNull, out IntPtr result);

        [DllImport("d3d9.dll")]
        private static extern int Direct3DCreate9Ex(uint SDK_VERSION, out IntPtr result);
    }
}

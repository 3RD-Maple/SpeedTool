using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    public class D3D9 : IDisposable
    {
        public unsafe D3D9()
        {
            d3d = Direct3DCreate9(D3DHook.SDK_VERSION);
            vtbl = *(IntPtr*)d3d;
            NativeCreateDevice = Marshal.GetDelegateForFunctionPointer<CreateDevicePtr>(((IntPtr*)vtbl)[16]);
        }

        public D3D9Device CreateDevice(uint Adapter, int DeviceType, IntPtr hWnd, int flags, D3DHook.D3DPRESENT_PARAMETERS PresentationParams)
        {
            IntPtr result = IntPtr.Zero;
            int hresult = NativeCreateDevice(d3d, Adapter, DeviceType, hWnd, flags, ref PresentationParams, out result);
            if(result == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot create D3D Device");
            }

            return new D3D9Device(result);
        }

        public void Dispose()
        {
            if(d3d != IntPtr.Zero)
                Marshal.Release(d3d);
        }

        IntPtr d3d;
        IntPtr vtbl;

        private CreateDevicePtr NativeCreateDevice;

        delegate int CreateDevicePtr(IntPtr pThis, uint Adapter, int DeviceType, IntPtr hwnd, int flags, [In] ref D3DHook.D3DPRESENT_PARAMETERS pPresentationParams, out IntPtr result);

        [DllImport("d3d9.dll")]
        private static extern IntPtr Direct3DCreate9(uint SDK_VERSION);
    }
}

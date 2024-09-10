using System;
using System.Runtime.InteropServices;

using InjectedTimer.D3D.DXGI;

namespace InjectedTimer.D3D.D3D11
{
    public class D3D11 : IDisposable
    {
        public D3D11(IntPtr window)
        {
            var desc = new DXGI_SWAP_CHAIN_DESC()
            {
                BufferDesc = new DXGI_MODE_DESC(),
                SampleDesc = new DXGI_SAMPLE_DESC(),
                BufferUsage = 0,
                BufferCount = 2,
                OutputWindow = window,
                Windowed = 1,
                SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD
            };

            int featureLevel;
            D3D11CreateDeviceAndSwapChain(IntPtr.Zero, (int)D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_NULL, IntPtr.Zero, 0x20, null, 0, D3D11_SDK_VERSION, ref desc, out swapChain, out device, out featureLevel, out context);
            if(swapChain == null)
            {
                throw new Exception("Could not create DX11");
            }
        }

        public unsafe IntPtr PresentPtr
        {
            get
            {
                var vtbl = *(IntPtr*)swapChain;
                return ((IntPtr*)vtbl)[9];
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        IntPtr device;
        IntPtr swapChain;
        IntPtr context;

        [DllImport("d3d11.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int D3D11CreateDeviceAndSwapChain([In]     IntPtr pAdapter,
                                                                         int DriverType,
                                                                         IntPtr Software,
                                                                         uint Flags,
                                                                [In]     int[] pFeatureLevels,
                                                                         uint FeatureLevels,
                                                                         uint SDKVersion,
                                                                [In]  ref DXGI_SWAP_CHAIN_DESC pSwapChainDesc,
                                                                [Out] out IntPtr ppSwapChain,
                                                                [Out] out IntPtr ppDevice,
                                                                [Out] out int pFeatureLevel,
                                                                [Out] out IntPtr ppImmediateContext);

        private const int D3D11_SDK_VERSION = 7;
    }
}

using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D.DXGI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct DXGI_SWAP_CHAIN_DESC
    {
        public DXGI_MODE_DESC BufferDesc;
        public DXGI_SAMPLE_DESC SampleDesc;
        public int BufferUsage;
        public uint BufferCount;
        public IntPtr OutputWindow;
        public int Windowed;

        [MarshalAs(UnmanagedType.I4)]
        public DXGI_SWAP_EFFECT SwapEffect;
        public uint Flags;
    }
}

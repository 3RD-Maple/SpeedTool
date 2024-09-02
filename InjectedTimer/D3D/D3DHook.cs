using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    public class D3DHook
    {
        public const uint SDK_VERSION = 32;

        public const int D3DDEVTYPE_HAL = 1;

        public const int D3DSWAPEFFECT_DISCARD = 1;

        public const int D3DCREATE_SOFTWARE_VERTEXPROCESSING = 0x00000020;
        public const int D3DCREATE_HARDWARE_VERTEXPROCESSING = 0x00000040;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct D3DPRESENT_PARAMETERS
        {
            public uint                BackBufferWidth;
            public uint                BackBufferHeight;
            public int           BackBufferFormat;
            public uint                BackBufferCount;
            public int  MultiSampleType;
            public uint               MultiSampleQuality;
            public int       SwapEffect;
            public IntPtr                hDeviceWindow;
            public uint                Windowed;
            public uint                EnableAutoDepthStencil;
            public int           AutoDepthStencilFormat;
            public uint               Flags;
            public uint                FullScreen_RefreshRateInHz;
            public uint                PresentationInterval;
        }
    }
}
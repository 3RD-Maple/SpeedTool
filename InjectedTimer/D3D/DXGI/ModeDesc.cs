using System.Runtime.InteropServices;

namespace InjectedTimer.D3D.DXGI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct DXGI_MODE_DESC
    {
        public DXGI_MODE_DESC() { }
        public uint                     Width = 0;
        public uint                     Height = 0;
        DXGI_RATIONAL            RefreshRate = new DXGI_RATIONAL();

        [MarshalAs(UnmanagedType.I4)]
        DXGI_FORMAT              Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

        [MarshalAs(UnmanagedType.I4)]
        DXGI_MODE_SCANLINE_ORDER ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;

        [MarshalAs(UnmanagedType.I4)]
        DXGI_MODE_SCALING        Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
    }
}

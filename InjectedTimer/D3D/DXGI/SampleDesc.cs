using System.Runtime.InteropServices;

namespace InjectedTimer.D3D.DXGI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct DXGI_SAMPLE_DESC
    {
        public DXGI_SAMPLE_DESC() { }
        public uint Count = 1;
        public uint Quality = 0;
    }
}

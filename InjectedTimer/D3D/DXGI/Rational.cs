using System.Runtime.InteropServices;

namespace InjectedTimer.D3D.DXGI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    struct DXGI_RATIONAL
    {
        public DXGI_RATIONAL() { }
        public uint Numerator = 0;
        public uint Denominator = 0;
    }
}

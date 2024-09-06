using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    public struct WNDCLASSA
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbtBackground;
        public string lpszMenuName;
        public string lpszClassName;
    }
}

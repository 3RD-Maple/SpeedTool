using System;
using System.Runtime.InteropServices;
using EasyHook;

namespace InjectedTimer.D3D
{
    public sealed class TempWindow : IDisposable
    {
        public TempWindow(string classname)
        {
            var wndclass = new WNDCLASSA()
            {
                lpfnWndProc = LocalHook.GetProcAddress("user32.dll", "DefWindowProcA"),
                lpszClassName = classname
            };

            if(!RegisterClass(ref wndclass))
                throw new Exception("Couldn't register class");

            HWND = CreateWindowEx(0, classname, classname, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            this.classname = classname;
            {
                
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool RegisterClass([In] ref WNDCLASSA lpWndClass);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool UnregisterClass(string lpWndClass, IntPtr hInstnace);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr lParam, IntPtr wParam);

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(
           int dwExStyle,
           string lpClassName,
           string lpWindowName,
           int dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        public IntPtr HWND { get; private set; }

        string classname;

        public void Dispose()
        {
            DestroyWindow(HWND);
            UnregisterClass(classname, IntPtr.Zero);
        }
    }
}

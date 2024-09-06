using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace InjectedTimer
{
    public static class APIHelper
    {
        public static IntPtr CurrentProcess
        {
            get
            {
                if(currentProcess != IntPtr.Zero)
                    return currentProcess;
                
                currentProcess = GetCurrentProcess();
                return currentProcess;
            }
        }

        public static IntPtr BaseModuleAddress
        {
            get
            {
                return Process.GetCurrentProcess().MainModule.BaseAddress;
            }
        }

        public static int ReadInt32(IntPtr address)
        {
            byte[] data = new byte[4];
            int read = 0;
            ReadProcessMemory(CurrentProcess, address, data, 4, out read);
            return BitConverter.ToInt32(data, 0);
        }

        public static byte[] ReadRaw(IntPtr address, int count)
        {
            byte[] data = new byte[count];
            int read = 0;
            ReadProcessMemory(CurrentProcess, address, data, count, out read);
            if(read != count)
                throw new Exception("Couldn't read enough");
            return data;
        }

        public static IntPtr GetModuleBaseAddress(string name)
        {
            var searchName = name.ToLower();
            for(int i = 0; i < Process.GetCurrentProcess().Modules.Count; i++)
            {
                var mod = Process.GetCurrentProcess().Modules[i];
                if(mod.ModuleName.ToLower() == searchName)
                    return mod.BaseAddress;
            }

            return IntPtr.Zero;
        }

        public static bool HasModule(string name)
        {
            var searchName = name.ToLower();
            for(int i = 0; i < Process.GetCurrentProcess().Modules.Count; i++)
            {
                var mod = Process.GetCurrentProcess().Modules[i];
                if(mod.ModuleName.ToLower() == searchName)
                    return true;
            }

            return false;
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            GetWindowText(hWnd, sb, 255);
            return sb.ToString();
        }

        public static List<IntPtr> GetCurrentProcessWindows()
        {
            var proc = GetCurrentProcessId();
            List<IntPtr> windows = new List<IntPtr>();
            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {

                int processId;
                GetWindowThreadProcessId(hWnd, out processId);
                if(processId == proc)
                    windows.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return windows;
        }

        /// <summary>
        /// Despite its name, returns _any_ window associated with this process. Possibly, just for now :)
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMainWindowHWND()
        {
            var proc = GetCurrentProcessId();
            IntPtr result = IntPtr.Zero;
            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {
                int processId;
                GetWindowThreadProcessId(hWnd, out processId);
                if(processId == proc)
                {
                    result = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return result;
        }

        private static IntPtr currentProcess = IntPtr.Zero;

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ReadProcessMemory(IntPtr hProcess,
          IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
    }
}

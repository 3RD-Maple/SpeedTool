using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        private static IntPtr currentProcess;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ReadProcessMemory(IntPtr hProcess,
          IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
    }
}

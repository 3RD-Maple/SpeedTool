using System;
using System.Linq;
using System.Text;

namespace InjectedTimer
{
    public static class ScriptFunction
    {
        public static long GetModuleAddress(string name)
        {
            return (long)APIHelper.GetModuleBaseAddress(name);
        }

        public static void DebugMessage(string message)
        {
            Sink.SendString("debug_message " + message);
        }

        public static void DebugMessageAddress(long addr)
        {
            Sink.SendString("debug_message 0x" + addr.ToString(IntPtr.Size == 8 ? "X16" : "X8"));
        }

        public static void SetLoading()
        {
            IsLoading = true;
        }

        public static void SetNotLoading()
        {
            IsLoading = false;
        }

        public static int ReadInt32(long addr)
        {
            return APIHelper.ReadInt32((IntPtr)addr);
        }

        public static long ReadInt64(long addr)
        {
            return APIHelper.ReadInt64((IntPtr)addr);
        }

        public static float ReadFloat(long addr)
        {
            return APIHelper.ReadSingle((IntPtr)addr);
        }

        public static double ReadDouble(long addr)
        {
            return APIHelper.ReadDouble((IntPtr)addr);
        }

        public static byte[] ReadBytes(long addr, int amount)
        {
            return APIHelper.ReadRaw((IntPtr)addr, amount);
        }

        public static long PointerPath(params long[] addresses)
        {
            Func<IntPtr, long> ReadAddr = (IntPtr addr) => IntPtr.Size == 4 ? APIHelper.ReadInt32(addr) : APIHelper.ReadInt64(addr);
            var len = addresses.Length;
            var begin = ReadAddr((IntPtr)addresses[0]);
            for(int i = 1; i < len - 1; i++)
                begin = ReadAddr((IntPtr)(begin + addresses[i]));

            return begin + addresses.Last();
        }

        public static void Split()
        {
            Sink.SendString("split");
        }

        public static void Start()
        {
            Sink.SendString("start");
        }

        public static string ReadASCII(long addr, int count)
        {
            try
            {
                var data = APIHelper.ReadRaw((IntPtr)addr, count);
                return Encoding.ASCII.GetString(data.Take(ASCIILength(data)).ToArray());
            }
            catch
            {
                return "";
            }
        }

        private static int ASCIILength(byte[] data)
        {
            int i = 0;
            while(data[i] != 0) i++;
            return i;
        }

        public static Pipe Sink;

        public static bool IsLoading = false;
    }
}

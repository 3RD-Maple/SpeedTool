using System;
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
                return Encoding.ASCII.GetString(data);
            }
            catch
            {
                return "";
            }
        }

        public static Pipe Sink;

        public static bool IsLoading = false;
    }
}

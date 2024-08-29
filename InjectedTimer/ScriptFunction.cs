using System;

namespace InjectedTimer
{
    public static class ScriptFunction
    {
        public static IntPtr GetModuleAddress(string name)
        {
            return APIHelper.GetModuleBaseAddress(name);
        }

        public static void DebugMessage(string message)
        {
            Sink.SendString("debug_message " + message);
        }

        public static void DebugMessageAddress(IntPtr addr)
        {
            Sink.SendString("debug_message 0x" + addr.ToString(IntPtr.Size == 8 ? "X16" : "X8"));
        }

        public static Pipe Sink;
    }
}

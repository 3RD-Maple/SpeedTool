using System;
using System.Reflection;
using NLua;

namespace InjectedTimer
{
    public sealed class ScriptEngine
    {
        public ScriptEngine(string script, Pipe sink)
        {
            luaState = new Lua(false);
            ScriptFunction.Sink = sink;
            luaState.DoString("import = function () end");

            luaState.RegisterFunction("module_base_address", ((Func<string, IntPtr>)ScriptFunction.GetModuleAddress).GetMethodInfo());
            luaState.RegisterFunction("debug_message", ((Action<string>)ScriptFunction.DebugMessage).GetMethodInfo());
            luaState.RegisterFunction("debug_message_address", ((Action<IntPtr>)ScriptFunction.DebugMessageAddress).GetMethodInfo());
            luaState.RegisterFunction("timer_set_loading", ((Action)ScriptFunction.SetLoading).GetMethodInfo());
            luaState.RegisterFunction("timer_set_not_loading", ((Action)ScriptFunction.SetNotLoading).GetMethodInfo());

            luaState.RegisterFunction("read_int32", ((Func<IntPtr, int>)ScriptFunction.ReadInt32).GetMethodInfo());

            luaState.DoString(script);
            luaState.DoString("on_load()");
            luaState.DoString("");
        }

        public void OnFrame()
        {
            luaState.DoString("on_frame()");
        }

        Lua luaState;
    }
}
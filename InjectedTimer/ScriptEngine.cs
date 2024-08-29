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

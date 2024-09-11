using System;
using System.Reflection;
using NLua;

namespace InjectedTimer
{
    public sealed class ScriptEngine : IDisposable
    {
        public ScriptEngine(string script, Pipe sink)
        {
            luaState = new Lua(true);
            ScriptFunction.Sink = sink;
            
            luaState.DoString(SANDBOX);

            luaState.RegisterFunction("module_base_address", ((Func<string, long>)ScriptFunction.GetModuleAddress).GetMethodInfo());
            luaState.RegisterFunction("debug_message", ((Action<string>)ScriptFunction.DebugMessage).GetMethodInfo());
            luaState.RegisterFunction("debug_message_address", ((Action<long>)ScriptFunction.DebugMessageAddress).GetMethodInfo());

            luaState.RegisterFunction("timer_set_loading", ((Action)ScriptFunction.SetLoading).GetMethodInfo());
            luaState.RegisterFunction("timer_set_not_loading", ((Action)ScriptFunction.SetNotLoading).GetMethodInfo());
            luaState.RegisterFunction("timer_split", ((Action)ScriptFunction.Split).GetMethodInfo());
            luaState.RegisterFunction("timer_start", ((Action)ScriptFunction.Start).GetMethodInfo());

            luaState.RegisterFunction("read_int", ((Func<long, int>)ScriptFunction.ReadInt32).GetMethodInfo());
            luaState.RegisterFunction("read_long", ((Func<long, long>)ScriptFunction.ReadInt64).GetMethodInfo());
            luaState.RegisterFunction("read_float", ((Func<long, float>)ScriptFunction.ReadFloat).GetMethodInfo());
            luaState.RegisterFunction("read_double", ((Func<long, double>)ScriptFunction.ReadDouble).GetMethodInfo());
            luaState.RegisterFunction("read_ascii", ((Func<long, int, string>)ScriptFunction.ReadASCII).GetMethodInfo());
            luaState.RegisterFunction("read_bytes", ((Func<long, int, byte[]>)ScriptFunction.ReadBytes).GetMethodInfo());
            luaState.RegisterFunction("pointer_path", ((Func<long[], long>)ScriptFunction.PointerPath).GetMethodInfo());

            luaState.DoString(script);
            luaState.DoString("run_sandbox(sandbox_env, on_load)");
            luaState.DoString("");
        }

        public void OnFrame()
        {
            luaState.DoString("run_sandbox(sandbox_env, on_frame)");
        }

        public void Dispose()
        {
            luaState.Dispose();
        }

        Lua luaState;

        private const string SANDBOX =
"""
-- save a pointer to globals that would be unreachable in sandbox
local e=_ENV

-- sample sandbox environment
sandbox_env = {
  ipairs = ipairs,
  next = next,
  pairs = pairs,
  pcall = pcall,
  tonumber = tonumber,
  tostring = tostring,
  type = type,
  unpack = unpack,
  coroutine = { create = coroutine.create, resume = coroutine.resume, 
      running = coroutine.running, status = coroutine.status, 
      wrap = coroutine.wrap },
  string = { byte = string.byte, char = string.char, find = string.find, 
      format = string.format, gmatch = string.gmatch, gsub = string.gsub, 
      len = string.len, lower = string.lower, match = string.match, 
      rep = string.rep, reverse = string.reverse, sub = string.sub, 
      upper = string.upper },
  table = { insert = table.insert, maxn = table.maxn, remove = table.remove, 
      sort = table.sort },
  math = { abs = math.abs, acos = math.acos, asin = math.asin, 
      atan = math.atan, atan2 = math.atan2, ceil = math.ceil, cos = math.cos, 
      cosh = math.cosh, deg = math.deg, exp = math.exp, floor = math.floor, 
      fmod = math.fmod, frexp = math.frexp, huge = math.huge, 
      ldexp = math.ldexp, log = math.log, log10 = math.log10, max = math.max, 
      min = math.min, modf = math.modf, pi = math.pi, pow = math.pow, 
      rad = math.rad, random = math.random, sin = math.sin, sinh = math.sinh, 
      sqrt = math.sqrt, tan = math.tan, tanh = math.tanh },
  os = { clock = os.clock, difftime = os.difftime, time = os.time },
}

function run_sandbox(sb_env, sb_func, ...)
  local sb_orig_env=_ENV
  if (not sb_func) then return nil end
  _ENV=sb_env
  local sb_ret={e.pcall(sb_func, ...)}
  _ENV=sb_orig_env
  return e.table.unpack(sb_ret)
end
""";
    }
}

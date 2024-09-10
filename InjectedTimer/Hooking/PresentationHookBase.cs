using System;

namespace InjectedTimer.Hooking
{
    public abstract class PresentationHookBase : IDisposable
    {
        public Action OnFrame;
        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        public abstract string HookName { get; }

        protected virtual void Release() { }

        public static PresentationHookBase AutoDetectCreateHook()
        {
            if(APIHelper.HasModule("d3d11.dll"))
                return new DirectX11Hook();
            if(APIHelper.HasModule("d3d9.dll"))
                return new DirectX9Hook();
            if(APIHelper.HasModule("GDI32.dll"))
                return new SwapBuffersHook();

            throw new Exception("Autodetect failed");
        }
    }
}

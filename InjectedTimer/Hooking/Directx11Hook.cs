using System;
using InjectedTimer.D3D;
using InjectedTimer.D3D.D3D11;

namespace InjectedTimer.Hooking
{
    public sealed class DirectX11Hook : PresentationHookBase
    {
        internal DirectX11Hook()
        {
            using(var wind = new TempWindow("SpeedToolD3D11"))
            {
                using(var d11 = new D3D11(wind.HWND))
                {
                    presentHook = new HookedFunction<D3D11.Present_Delegate>(d11.PresentPtr, MyPresent);
                }
            }
        }

        public override string HookName => "DirectX 11";

        protected override void Release()
        {
            presentHook.Dispose();
            base.Release();
        }

        int MyPresent(IntPtr pThis, uint SyncInterval, uint Flags)
        {
            OnFrame?.Invoke();
            return presentHook.Original(pThis, SyncInterval, Flags);
        }

        HookedFunction<D3D11.Present_Delegate> presentHook;
    }
}
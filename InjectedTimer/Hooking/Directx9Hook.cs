using System;
using InjectedTimer.D3D;

namespace InjectedTimer.Hooking
{
    public sealed class DirectX9Hook : PresentationHookBase
    {
        internal DirectX9Hook()
        {
            using(var wnd = new TempWindow("SpeedToolHookWindow"))
            {
                var hWnd = wnd.HWND;
                var para = new D3DHook.D3DPRESENT_PARAMETERS()
                {
                    Windowed = 1,
                    SwapEffect = D3DHook.D3DSWAPEFFECT_DISCARD,
                    hDeviceWindow = hWnd
                };

                using(var D3D = new D3D9())
                {
                    using(var device = D3D.CreateDevice(0, D3DHook.D3DDEVTYPE_NULLREF, hWnd, D3DHook.D3DCREATE_HARDWARE_VERTEXPROCESSING, para))
                    {
                        presentHook = new HookedFunction<D3D9Device.Present_Delegate>(device.PresentFuncPtr, new D3D9Device.Present_Delegate(MyPresent));
                    }
                }
                using(var D3DEx = new D3D9Ex())
                {
                    using(var device = D3DEx.CreateDeviceEx(0, D3DHook.D3DDEVTYPE_NULLREF, hWnd, D3DHook.D3DCREATE_HARDWARE_VERTEXPROCESSING, para))
                    {
                        presentExHook = new HookedFunction<D3D9DeviceEx.PresentEx_Delegate>(device.PresentExFuncPtr, new D3D9DeviceEx.PresentEx_Delegate(MyPresentEx));
                    }
                }
            }
        }

        public override string HookName => "DirectX 9";

        protected override void Release()
        {
            presentExHook.Dispose();
            presentHook.Dispose();
            base.Release();
        }

        HookedFunction<D3D9Device.Present_Delegate> presentHook;
        HookedFunction<D3D9DeviceEx.PresentEx_Delegate> presentExHook;

        int MyPresent(IntPtr pThis, IntPtr pSourceRect, IntPtr pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion)
        {
            OnFrame?.Invoke();
            return presentHook.Original(pThis, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
        }

        int MyPresentEx(IntPtr pThis, IntPtr pSourceRect, IntPtr pDestRect, IntPtr hDestWindowOverride, IntPtr pDirtyRegion, int flags)
        {
            OnFrame?.Invoke();
            return presentExHook.Original(pThis, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, flags);
        }

    }
}

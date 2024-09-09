using System;
using System.Runtime.InteropServices;

namespace InjectedTimer.D3D.D3D11
{
    public class D3D11 : IDisposable
    {
        public D3D11(IntPtr window)
        {
            var desc = new DXGI_SWAP_CHAIN_DESC()
            {
                BufferDesc = new DXGI_MODE_DESC(),
                SampleDesc = new DXGI_SAMPLE_DESC(),
                BufferUsage = 0,
                BufferCount = 2,
                OutputWindow = window,
                Windowed = 1,
                SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD
            };

            int[] featureLevels = new int[]{0};

            IntPtr temp = IntPtr.Zero;
            int featureLevel;
            D3D11CreateDeviceAndSwapChain(ref temp, (int)D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_NULL, IntPtr.Zero, 0, featureLevels, 1, D3DHook.SDK_VERSION, ref desc, out swapChain, out device, out featureLevel, out context);
            if(swapChain == null)
            {
                throw new Exception("Could not create DX11");
            }
        }

        public unsafe IntPtr PresentPtr
        {
            get
            {
                var vtbl = *(IntPtr*)swapChain;
                return ((IntPtr*)vtbl)[9];
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        IntPtr device;
        IntPtr swapChain;
        IntPtr context;

        [DllImport("d3d11.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int D3D11CreateDeviceAndSwapChain([In]  ref IntPtr pAdapter,
                                                                         int DriverType,
                                                                         IntPtr Software,
                                                                         uint Flags,
                                                                [In]     int[] pFeatureLevels,
                                                                         uint FeatureLevels,
                                                                         uint SDKVersion,
                                                                [In]  ref DXGI_SWAP_CHAIN_DESC pSwapChainDesc,
                                                                [Out] out IntPtr ppSwapChain,
                                                                [Out] out IntPtr ppDevice,
                                                                [Out] out int pFeatureLevel,
                                                                [Out] out IntPtr ppImmediateContext);

        struct DXGI_SWAP_CHAIN_DESC
        {
            public DXGI_MODE_DESC BufferDesc;
            public DXGI_SAMPLE_DESC SampleDesc;
            public int BufferUsage;
            public uint BufferCount;
            public IntPtr OutputWindow;
            public int Windowed;

            [MarshalAs(UnmanagedType.I4)]
            public DXGI_SWAP_EFFECT SwapEffect;
            public uint Flags;
        }

        struct DXGI_MODE_DESC
        {
            public DXGI_MODE_DESC() { }
            public uint                     Width = 0;
            public uint                     Height = 0;
            DXGI_RATIONAL            RefreshRate = new DXGI_RATIONAL();

            [MarshalAs(UnmanagedType.I4)]
            DXGI_FORMAT              Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

            [MarshalAs(UnmanagedType.I4)]
            DXGI_MODE_SCANLINE_ORDER ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;

            [MarshalAs(UnmanagedType.I4)]
            DXGI_MODE_SCALING        Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
        }

        enum DXGI_MODE_SCALING
        { 
            DXGI_MODE_SCALING_UNSPECIFIED  = 0,
            DXGI_MODE_SCALING_CENTERED     = 1,
            DXGI_MODE_SCALING_STRETCHED    = 2
        }

        enum DXGI_MODE_SCANLINE_ORDER
        { 
            DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED        = 0,
            DXGI_MODE_SCANLINE_ORDER_PROGRESSIVE        = 1,
            DXGI_MODE_SCANLINE_ORDER_UPPER_FIELD_FIRST  = 2,
            DXGI_MODE_SCANLINE_ORDER_LOWER_FIELD_FIRST  = 3
        }

        enum DXGI_FORMAT
        {
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
            DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
            DXGI_FORMAT_R32G32B32A32_UINT = 3,
            DXGI_FORMAT_R32G32B32A32_SINT = 4,
            DXGI_FORMAT_R32G32B32_TYPELESS = 5,
            DXGI_FORMAT_R32G32B32_FLOAT = 6,
            DXGI_FORMAT_R32G32B32_UINT = 7,
            DXGI_FORMAT_R32G32B32_SINT = 8,
            DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
            DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
            DXGI_FORMAT_R16G16B16A16_UNORM = 11,
            DXGI_FORMAT_R16G16B16A16_UINT = 12,
            DXGI_FORMAT_R16G16B16A16_SNORM = 13,
            DXGI_FORMAT_R16G16B16A16_SINT = 14,
            DXGI_FORMAT_R32G32_TYPELESS = 15,
            DXGI_FORMAT_R32G32_FLOAT = 16,
            DXGI_FORMAT_R32G32_UINT = 17,
            DXGI_FORMAT_R32G32_SINT = 18,
            DXGI_FORMAT_R32G8X24_TYPELESS = 19,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
            DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
            DXGI_FORMAT_R10G10B10A2_UNORM = 24,
            DXGI_FORMAT_R10G10B10A2_UINT = 25,
            DXGI_FORMAT_R11G11B10_FLOAT = 26,
            DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
            DXGI_FORMAT_R8G8B8A8_UNORM = 28,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
            DXGI_FORMAT_R8G8B8A8_UINT = 30,
            DXGI_FORMAT_R8G8B8A8_SNORM = 31,
            DXGI_FORMAT_R8G8B8A8_SINT = 32,
            DXGI_FORMAT_R16G16_TYPELESS = 33,
            DXGI_FORMAT_R16G16_FLOAT = 34,
            DXGI_FORMAT_R16G16_UNORM = 35,
            DXGI_FORMAT_R16G16_UINT = 36,
            DXGI_FORMAT_R16G16_SNORM = 37,
            DXGI_FORMAT_R16G16_SINT = 38,
            DXGI_FORMAT_R32_TYPELESS = 39,
            DXGI_FORMAT_D32_FLOAT = 40,
            DXGI_FORMAT_R32_FLOAT = 41,
            DXGI_FORMAT_R32_UINT = 42,
            DXGI_FORMAT_R32_SINT = 43,
            DXGI_FORMAT_R24G8_TYPELESS = 44,
            DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
            DXGI_FORMAT_R8G8_TYPELESS = 48,
            DXGI_FORMAT_R8G8_UNORM = 49,
            DXGI_FORMAT_R8G8_UINT = 50,
            DXGI_FORMAT_R8G8_SNORM = 51,
            DXGI_FORMAT_R8G8_SINT = 52,
            DXGI_FORMAT_R16_TYPELESS = 53,
            DXGI_FORMAT_R16_FLOAT = 54,
            DXGI_FORMAT_D16_UNORM = 55,
            DXGI_FORMAT_R16_UNORM = 56,
            DXGI_FORMAT_R16_UINT = 57,
            DXGI_FORMAT_R16_SNORM = 58,
            DXGI_FORMAT_R16_SINT = 59,
            DXGI_FORMAT_R8_TYPELESS = 60,
            DXGI_FORMAT_R8_UNORM = 61,
            DXGI_FORMAT_R8_UINT = 62,
            DXGI_FORMAT_R8_SNORM = 63,
            DXGI_FORMAT_R8_SINT = 64,
            DXGI_FORMAT_A8_UNORM = 65,
            DXGI_FORMAT_R1_UNORM = 66,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
            DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
            DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
            DXGI_FORMAT_BC1_TYPELESS = 70,
            DXGI_FORMAT_BC1_UNORM = 71,
            DXGI_FORMAT_BC1_UNORM_SRGB = 72,
            DXGI_FORMAT_BC2_TYPELESS = 73,
            DXGI_FORMAT_BC2_UNORM = 74,
            DXGI_FORMAT_BC2_UNORM_SRGB = 75,
            DXGI_FORMAT_BC3_TYPELESS = 76,
            DXGI_FORMAT_BC3_UNORM = 77,
            DXGI_FORMAT_BC3_UNORM_SRGB = 78,
            DXGI_FORMAT_BC4_TYPELESS = 79,
            DXGI_FORMAT_BC4_UNORM = 80,
            DXGI_FORMAT_BC4_SNORM = 81,
            DXGI_FORMAT_BC5_TYPELESS = 82,
            DXGI_FORMAT_BC5_UNORM = 83,
            DXGI_FORMAT_BC5_SNORM = 84,
            DXGI_FORMAT_B5G6R5_UNORM = 85,
            DXGI_FORMAT_B5G5R5A1_UNORM = 86,
            DXGI_FORMAT_B8G8R8A8_UNORM = 87,
            DXGI_FORMAT_B8G8R8X8_UNORM = 88,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
            DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
            DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
            DXGI_FORMAT_BC6H_TYPELESS = 94,
            DXGI_FORMAT_BC6H_UF16 = 95,
            DXGI_FORMAT_BC6H_SF16 = 96,
            DXGI_FORMAT_BC7_TYPELESS = 97,
            DXGI_FORMAT_BC7_UNORM = 98,
            DXGI_FORMAT_BC7_UNORM_SRGB = 99,
            DXGI_FORMAT_AYUV = 100,
            DXGI_FORMAT_Y410 = 101,
            DXGI_FORMAT_Y416 = 102,
            DXGI_FORMAT_NV12 = 103,
            DXGI_FORMAT_P010 = 104,
            DXGI_FORMAT_P016 = 105,
            DXGI_FORMAT_420_OPAQUE = 106,
            DXGI_FORMAT_YUY2 = 107,
            DXGI_FORMAT_Y210 = 108,
            DXGI_FORMAT_Y216 = 109,
            DXGI_FORMAT_NV11 = 110,
            DXGI_FORMAT_AI44 = 111,
            DXGI_FORMAT_IA44 = 112,
            DXGI_FORMAT_P8 = 113,
            DXGI_FORMAT_A8P8 = 114,
            DXGI_FORMAT_B4G4R4A4_UNORM = 115,
            DXGI_FORMAT_P208 = 130,
            DXGI_FORMAT_V208 = 131,
            DXGI_FORMAT_V408 = 132,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIN_MIP_OPAQUE,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE,
            DXGI_FORMAT_FORCE_UINT = -1
        }

        struct DXGI_RATIONAL
        {
            public DXGI_RATIONAL() { }
            public uint Numerator = 0;
            public uint Denominator = 0;
        }

        enum D3D_DRIVER_TYPE
        {
            D3D_DRIVER_TYPE_UNKNOWN = 0,
            D3D_DRIVER_TYPE_HARDWARE,
            D3D_DRIVER_TYPE_REFERENCE,
            D3D_DRIVER_TYPE_NULL,
            D3D_DRIVER_TYPE_SOFTWARE,
            D3D_DRIVER_TYPE_WARP
        }

        struct DXGI_SAMPLE_DESC
        {
            public DXGI_SAMPLE_DESC() { }
            public uint Count = 1;
            public uint Quality = 0;
        }

        enum DXGI_SWAP_EFFECT
        {
            DXGI_SWAP_EFFECT_DISCARD = 0,
            DXGI_SWAP_EFFECT_SEQUENTIAL = 1,
            DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL = 3,
            DXGI_SWAP_EFFECT_FLIP_DISCARD = 4
        }
    }
}

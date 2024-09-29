using System.Runtime.InteropServices;
using Windows.Win32;

namespace Hardware.Info.Aot.Windows
{
    class Constants
    {
        public static readonly Guid CLSID_WbemLocator = new Guid(
            0x4590f811, 0x1d3a, 0x11d0, 0x89, 0x1f, 0x00, 0xaa, 0x00, 0x4b, 0x2e, 0x24
        );

        public static readonly Guid IID_IWbemLocator = new Guid(
            0xdc12a687, 0x737f, 0x11cf, 0x88, 0x4d, 0x00, 0xaa, 0x00, 0x4b, 0x2e, 0x24
        );

        public const int WBEM_INFINITE = unchecked((int)0xFFFFFFFF);
        public const int RPC_C_AUTHN_WINNT = 10;
        public const int RPC_C_AUTHZ_NONE = 0;

        public const int WBEM_E_NOT_FOUND = unchecked((int)0x80041002);

        public static readonly SafeHandle NullSafeHandle = new SysFreeStringSafeHandle(IntPtr.Zero, false);
    }
}

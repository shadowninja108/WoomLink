using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResAction
    {
        public Pointer<char> Name;
#if XLINK_VER_BLITZ || XLINK_VER_PARK
        public uint TriggerStartIdx;
        public uint TriggerEndIdx;
#elif XLINK_VER_THUNDER || XLINK_VER_EXKING
        public ushort TriggerStartIdx;
        public ushort TriggerEndIdx;
#else
#error Invalid XLink version target.
#endif
    }
}

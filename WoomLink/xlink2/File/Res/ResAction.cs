using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResAction
    {
        public Pointer<char> Name;
#if XLINK_VER_THUNDER
        public ushort TriggerStartIdx;
        public ushort TriggerEndIdx;
#elif XLINK_VER_BLITZ
        public uint TriggerStartIdx;
        public uint TriggerEndIdx;
#endif
    }
}

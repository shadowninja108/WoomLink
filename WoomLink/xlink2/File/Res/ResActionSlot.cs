using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResActionSlot
    {
        public Pointer<char> Name;
        public ushort ActionStartIdx;
        public ushort ActionEndIdx;
    }
}

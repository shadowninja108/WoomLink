using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public struct ResParam
    {
        [FieldOffset(0x0)]
        public UInt24 Value24;
        [FieldOffset(0x3)]
        public ValueReferenceType ReferenceType;

        public uint Value => Value24.Value;
    }
}

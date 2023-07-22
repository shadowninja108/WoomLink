using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResCallTable
    {
        public short EnumValue = -1;
        public sbyte Flags = 0;
        public byte Unk3;

        public ResCallTable() { }
    }
}

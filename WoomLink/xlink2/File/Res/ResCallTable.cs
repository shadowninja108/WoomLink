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

        public bool IsNeedFade
        {
            readonly get => (Flags & 0b1) != 0;
            set
            {
                if (value)
                    Flags |= 0b1;
                else
                    Flags &= ~0b1;
            }
        }

        public bool IsNeedObserve
        {
            readonly get => (Flags & 0b10) != 0;
            set
            {
                if (value)
                    Flags |= 0b10;
                else
                    Flags &= ~0b10;
            }
        }
    }
}

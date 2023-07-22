using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File
{
    [StructLayout(LayoutKind.Sequential, Size = 3, Pack = 1)]
    public struct UInt24
    {
        private byte Byte0;
        private byte Byte1;
        private byte Byte2;

        public uint Value
        {
            get => (uint)(Byte0 | (Byte1 << 8) | (Byte2 << 16));
            set
            {
                Byte0 = (byte)value;
                Byte1 = (byte)(value >> 8);
                Byte2 = (byte)(value >> 16);
            }
        }
    }
}

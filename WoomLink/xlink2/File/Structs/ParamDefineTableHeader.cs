using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x14)]
    public struct ParamDefineTableHeader
    {
        public uint Size;
        public uint NumUserParams;
        public uint NumAssetParams;
        public uint Unk;
        public uint NumTriggerParams;
    }
}

using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Structs
{

    [StructLayout(LayoutKind.Sequential, Size = 0xC)]
    public struct ParamDefine
    {
        public uint Name;
        public ParamType Type;
        public int DefaultValue;
    }

}

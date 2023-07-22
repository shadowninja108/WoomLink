using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResCurveCallTable
    {
        public ushort CurvePointStartPos;
        public ushort NumPoint;
        public ushort CurveType;
        public ushort IsPropGlobal;
        public Pointer<char> PropName;
        public uint PropIdx;
        public short LocalPropertyNameIdx;
        public ushort Padding;

        public Pointer<CurvePointTable> CurvePoint => Pointer<CurvePointTable>.As(CurvePointStartPos);
        public bool IsPropGlobalBool => IsPropGlobal != 0;
    }
}

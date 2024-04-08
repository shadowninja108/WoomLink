using WoomLink.Ex;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public struct EditorResourceParam
    {
        public CommonResourceParam Common;
        public UintPointer EditorBin;
        public uint EditorBinSize;
        public Pointer<ResUserHeader> UserHeader;
        public string Name;
        public bool Initialized;
        public UintPointer FieldF0;
        public UintPointer FieldF8;
    }
}

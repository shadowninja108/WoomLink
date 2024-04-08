using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResPropertyTrigger
    {
#if XLINK_VER_BLITZ || XLINK_VER_PARK
        public uint GuId;
        public Pointer<ResAssetCallTable> AssetCtb;
        public Pointer<ResCondition> Condition;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#elif XLINK_VER_THUNDER || XLINK_VER_EXKING
        public uint GuId;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResAssetCallTable> AssetCtb;
        public Pointer<ResCondition> Condition;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#else
#error Invalid XLink version target.
#endif
    }
}
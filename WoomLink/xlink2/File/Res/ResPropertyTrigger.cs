using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResPropertyTrigger
    {
#if XLINK_VER_BLITZ
        public uint GuId;
        public Pointer<ResAssetCallTable> AssetCtb;
        public Pointer<ResCondition> Condition;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#elif XLINK_VER_THUNDER
        public uint GuId;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResAssetCallTable> AssetCtb;
        public Pointer<ResCondition> Condition;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#endif
    }
}
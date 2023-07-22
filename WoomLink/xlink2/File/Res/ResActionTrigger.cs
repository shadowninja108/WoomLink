using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResActionTrigger
    {
#if XLINK_VER_THUNDER
        public uint GuId;
        public int EndFrame;
        public Pointer<ResAssetCallTable> AssetCtbPos;
        public Pointer<char> String;
        public uint StartFrame;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#elif XLINK_VER_BLITZ
        public uint GuId;
        public Pointer<ResAssetCallTable> AssetCtbPos;
        public Pointer<char> String;
        public int EndFrame;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;

        public readonly uint StartFrame => String.PointerValue;
#endif

        public readonly ActionTriggerType TriggerType => ResourceUtil.GetActionTriggerType(in this);
    }
}

using System.Runtime.InteropServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResActionTrigger
    {
#if XLINK_VER_BLITZ || XLINK_VER_PARK
        public uint GuId;
        public Pointer<ResAssetCallTable> AssetCtbPos;
        public Pointer<char> String;
        public int EndFrame;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;

        /* Union? */
        public readonly uint StartFrame => String.PointerValue;
#elif XLINK_VER_THUNDER || XLINK_VER_EXKING
        public uint GuId;
        public int EndFrame;
        public Pointer<ResAssetCallTable> AssetCtbPos;
        public Pointer<char> String;
        public uint StartFrame;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;
#else
#error Invalid XLink version target.
#endif

        public readonly ActionTriggerType TriggerType => ResourceUtil.GetActionTriggerType(in this);
    }
}

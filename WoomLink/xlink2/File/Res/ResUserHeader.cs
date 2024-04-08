using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResUserHeader
    {
        public uint IsSetup;
#if XLINK_VER_BLITZ || XLINK_VER_PARK || XLINK_VER_THUNDER
        public int NumLocalProperty;
#elif XLINK_VER_EXKING
        public short NumLocalProperty;
        public short Unk;
#else
#error Invalid XLink version target.
#endif
        public int NumCallTable;
        public int NumAsset;
        public int NumRandomContainer;
        public int NumResActionSlot;
        public int NumResAction;
        public int NumResActionTrigger;
        public int NumResProperty;
        public int NumResPropertyTrigger;
        public int NumResAlwaysTrigger;
        public UintPointer TriggerTablePos;

        public bool IsSetupBool => IsSetup != 0;
    }
}

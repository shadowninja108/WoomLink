using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResUserHeader
    {
        public uint IsSetup;
        public int NumLocalProperty;
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

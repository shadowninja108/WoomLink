using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResPropertyTrigger
    {
        public uint GuId;
        public Pointer<ResAssetCallTable> AssetCtb;
        public int Condition;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResTriggerOverwriteParam> OverwriteParam;

        public Pointer<ResCondition> ConditionPtr => Pointer<ResCondition>.As((uint)Condition);
    }
}
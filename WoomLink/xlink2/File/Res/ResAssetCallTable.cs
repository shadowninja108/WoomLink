using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResAssetCallTable
    {
        public Pointer<char> KeyName;
        public ushort AssetId;
        public ushort Flag;
        public int Duration;
        public int ParentIndex;
        public ushort EnumIndex;
        public byte IsSolved;
        public uint KeyNameHash;
        public UintPointer ParamPos;
        public Pointer<ResCondition> Condition;

        public Pointer<ResContainerParam> ParamAsContainer => Pointer<ResContainerParam>.As(ParamPos);
        public Pointer<ResAssetParam> ParamAsAsset => Pointer<ResAssetParam>.As(ParamPos);

        public bool IsContainer => (Flag & 1) != 0;
        public bool IsSolvedBool => IsSolved != 0;
    }
}

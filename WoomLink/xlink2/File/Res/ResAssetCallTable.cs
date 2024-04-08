using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResAssetCallTable
    {
        public Pointer<char> KeyName;
        public short AssetId;
        public ushort Flag;
        public int Duration;
        public int ParentIndex;
        public ushort EnumIndex;
        public byte IsSolved;
        public uint KeyNameHash;
        public UintPointer ParamPos;
        public Pointer<ResCondition> Condition;

        public readonly Pointer<ResContainerParam> ParamAsContainer => Pointer<ResContainerParam>.As(ParamPos);
        public readonly Pointer<ResAssetParam> ParamAsAsset => Pointer<ResAssetParam>.As(ParamPos);

        public readonly bool IsContainer => (Flag & 1) != 0;
        public readonly bool IsSolvedBool => IsSolved != 0;
    }
}

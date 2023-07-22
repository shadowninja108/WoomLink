using System.Numerics;

namespace WoomLink.xlink2.File.Res
{
    public struct ResAssetParam
    {
        public ulong Bitfield;
        public int Count => BitOperations.PopCount(Bitfield);

        public readonly bool IsParamDefault(uint bit)
        {
            return (Bitfield & (1UL << (int)bit)) == 0;
        }
    }
}

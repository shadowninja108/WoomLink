using System.Numerics;

namespace WoomLink.xlink2.File.Res
{
    public struct ResTriggerOverwriteParam
    {
        public uint Bitfield;

        public int Count => BitOperations.PopCount(Bitfield);

        public bool IsParamDefault(uint bit)
        {
            return (Bitfield & 1UL << (int)bit) == 0;
        }
    }
}

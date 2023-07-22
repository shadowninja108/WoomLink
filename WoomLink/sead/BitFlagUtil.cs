using System.Numerics;
using System.Runtime.CompilerServices;

namespace WoomLink.sead
{
    public class BitFlagUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnBit(uint x)
        {
            return BitOperations.PopCount(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnBit64(ulong x)
        {
            return BitOperations.PopCount(x);
        }

        public static int CountRightOnBit(uint x, int bit)
        {
            uint mask = ((1u << bit) - 1) | (1u << bit);
            return CountOnBit(x & mask);
        }

        public static int CountRightOnBit64(ulong x, int bit)
        {
            ulong mask = ((1ul << bit) - 1) | (1ul << bit);
            return CountOnBit64(x & mask);
        }
    }
}

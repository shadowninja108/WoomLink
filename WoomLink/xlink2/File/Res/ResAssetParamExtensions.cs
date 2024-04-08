using System;
using System.Runtime.CompilerServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{
    public static class ResAssetParamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResParam> GetValues(this Pointer<ResAssetParam> pointer)
        {
            return pointer.AtEnd<ResParam>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<ResParam> GetValuesSpan(this Pointer<ResAssetParam> pointer)
        {
            return GetValues(pointer).AsSpan(pointer.Ref.Count);
        }
    }
}

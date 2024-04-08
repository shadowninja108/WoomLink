using System;
using System.Runtime.CompilerServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{
    public static class ResTriggerOverwriteParamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResParam> GetValues(this Pointer<ResTriggerOverwriteParam> pointer)
        {
            return pointer.AtEnd<ResParam>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<ResParam> GetValuesSpan(this Pointer<ResTriggerOverwriteParam> pointer)
        {
            return GetValues(pointer).AsSpan(pointer.Ref.Count);
        }
    }
}

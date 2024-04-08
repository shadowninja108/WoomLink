using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    public static class ResContainerParamExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResContainerParam.ForSwitch> GetForSwitch(this Pointer<ResContainerParam> pointer)
        {
            return pointer.AtEnd<ResContainerParam.ForSwitch>();
        }

#if XLINK_VER_THUNDER || XLINK_VER_EXKING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResContainerParam.ForGrid> GetForGrid(this Pointer<ResContainerParam> pointer)
        {
            Debug.Assert(pointer.Ref.Type == ContainerType.Grid);
            return pointer.AtEnd<ResContainerParam.ForGrid>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> GetProperty1ValueArray(this Pointer<ResContainerParam.ForGrid> pointer)
        {
            ref var param = ref pointer.Ref;
            var total = pointer.AtEnd<int>().AsSpan(param.Property1ValueArrayCount + param.Property2ValueArrayCount);

            return total.Slice(0, param.Property1ValueArrayCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<int> GetProperty2ValueArray(this Pointer<ResContainerParam.ForGrid> pointer)
        {
            ref var param = ref pointer.Ref;
            var total = pointer.AtEnd<int>().AsSpan(param.Property1ValueArrayCount + param.Property2ValueArrayCount);

            return total.Slice(param.Property1ValueArrayCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResContainerParam.Special> GetSpecial(this Pointer<ResContainerParam> pointer)
        {
            /* Field1 must be nonzero, but I have only observed Blend to behave differently when this is set. This may not be strictly correct. */
            Debug.Assert(pointer.Ref.Type == ContainerType.Blend && pointer.Ref.Field1 != 0);
            return pointer.AtEnd<ResContainerParam.Special>();
        }
#endif
    }
}

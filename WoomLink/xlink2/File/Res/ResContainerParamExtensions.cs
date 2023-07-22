using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ResContainerParam.ForMono> GetForMono(this Pointer<ResContainerParam> pointer)
        {
            Debug.Assert(pointer.Ref.Type == ContainerType.Mono);
            return pointer.AtEnd<ResContainerParam.ForMono>();
        }
    }
}

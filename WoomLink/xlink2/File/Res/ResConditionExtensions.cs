using System.Diagnostics;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;
using static WoomLink.xlink2.File.Res.ResCondition;

namespace WoomLink.xlink2.File.Res
{
    public static class ResConditionExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ForRandom> GetForRandom(this Pointer<ResCondition> pointer)
        {
            Debug.Assert(pointer.Ref.IsRandom);
            return pointer.AtEnd<ForRandom>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ForSequence> GetForSequence(this Pointer<ResCondition> pointer)
        {
            Debug.Assert(pointer.Ref.IsSequence);
            return pointer.AtEnd<ForSequence>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ForSwitch> GetForSwitch(this Pointer<ResCondition> pointer)
        {
            Debug.Assert(pointer.Ref.IsSwitch);
            return pointer.AtEnd<ForSwitch>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<ForBlend> GetForBlend(this Pointer<ResCondition> pointer)
        {
            Debug.Assert(pointer.Ref.IsBlend);
            return pointer.AtEnd<ForBlend>();
        }

        public static Pointer<ResCondition> GetNext(this Pointer<ForSwitch> pointer)
        {
            if (pointer.Ref.PropertyType != PropertyType.Enum || Arch.Is32())
                return pointer.AtEnd<ResCondition>();
            else
                return pointer.GetStringValue().AtEnd<ResCondition>();
        }
        
        public static Pointer<Pointer<char>> GetStringValue(this Pointer<ForSwitch> pointer)
        {
            Debug.Assert(pointer.Ref.PropertyType == PropertyType.Enum);
#if XLINK_ARCH_32
            return pointer.AddBytes(8).Cast<Pointer<char>>();
#elif XLINK_ARCH_64
            return pointer.AtEnd<Pointer<char>>();
#endif
        }


        public static Pointer<ResCondition> GetNext(this Pointer<ResCondition> pointer)
        {
            ref var condition = ref pointer.Ref;
            if (condition.IsRandom)
            {
                var next = pointer.GetForRandom().AtEnd<ResCondition>();
                return next;
            }
            if (condition.IsSequence)
            {
                var next = pointer.GetForSequence().AtEnd<ResCondition>();
                return next;
            }
            if (condition.IsSwitch)
            {
                var switchs = pointer.GetForSwitch();
                var next = switchs.GetNext();
                return next;
            }
            if (condition.IsBlend)
            {
                var next = pointer.GetForBlend().AtEnd<ResCondition>();
                return next;
            }

            Debug.Assert(false);

            /* Yes this is what they do. */
            return pointer;
        }
    }
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        public static Pointer<int> GetIntValue(this Pointer<ForSwitch> pointer)
        {
            Debug.Assert(pointer.Ref.PropertyType == PropertyType.S32);
            return pointer.AtEnd<int>();
        }

        public static Pointer<float> GetFloatValue(this Pointer<ForSwitch> pointer)
        {
            Debug.Assert(pointer.Ref.PropertyType == PropertyType.F32);
            return pointer.AtEnd<float>();
        }

        public static Pointer<ResCondition> GetNext(this Pointer<ResCondition> pointer)
        {
            ref var condition = ref pointer.Ref;
            if (condition.IsRandom)
            {
                var next = pointer.GetForRandom().AtEnd<ResCondition>();
                //Debug.Assert(next.PointerValue - pointer.PointerValue == 0x8);
                return next;
            }
            if (condition.IsSequence)
            {
                var next = pointer.GetForSequence().AtEnd<ResCondition>();
                //Debug.Assert(next.PointerValue - pointer.PointerValue == 0x8);
                return next;
            }
            if (condition.IsSwitch)
            {
                var switchs = pointer.GetForSwitch();
                var next = switchs.GetNext();
               //if(switchs.Ref.PropertyType == PropertyType.Enum)
               //    Debug.Assert(next.PointerValue - pointer.PointerValue == 0x18);
               //else
               //    Debug.Assert(next.PointerValue - pointer.PointerValue == 0x10);
                return next;
            }
            if (condition.IsBlend)
            {
                var next = pointer.GetForBlend().AtEnd<ResCondition>();
                //Debug.Assert(next.PointerValue - pointer.PointerValue == 0x10);
                return next;
            }

            /* Yes this is what they do. */
            Debug.Assert(false);
            return pointer;
        }
    }
}

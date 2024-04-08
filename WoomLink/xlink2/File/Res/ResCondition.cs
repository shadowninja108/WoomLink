using System;
using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    public struct ResCondition
    {
        public ContainerType Type;

        public readonly bool IsRandom => Type is ContainerType.Random or ContainerType.Random2;
        public readonly bool IsSequence => Type == ContainerType.Sequence;
        public readonly bool IsSwitch => Type == ContainerType.Switch;
        public readonly bool IsBlend => Type == ContainerType.Blend;


        [StructLayout(LayoutKind.Sequential)]
        public struct ForRandom
        {
            public float Weight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ForSequence
        {
            public int IsContinueOnFade;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ForSwitch
        {
#if XLINK_VER_BLITZ
            public PropertyType PropertyType;
            public CompareType CompareType;
            public uint SmallValue;
            public short LocalPropertyEnumNameIdx;
            public byte IsSolved;
            public byte IsGlobal;
#elif XLINK_VER_PARK
            public PropertyType PropertyType;
            public CompareType CompareType;
            public byte IsSolved;
            public byte IsGlobal;
            public uint SmallValue;
            public int LocalPropertyEnumNameIdx;
#elif XLINK_VER_THUNDER || XLINK_VER_EXKING
            public PropertyType PropertyType;
            public CompareType CompareType;
            public byte IsSolved;
            public byte IsGlobal;
            public int LocalPropertyEnumNameIdx;
            public uint SmallValue;
#else
#error Invalid XLink version target.
#endif

            public bool IsSolvedBool => IsSolved != 0;
            public bool IsGlobalBool => IsGlobal != 0;

            public int IntValue => (int)SmallValue;
            public float FloatValue => BitConverter.UInt32BitsToSingle(SmallValue);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ForBlend
        {
            public int Unk1;
            public int Unk2;
            public int Unk3;
        }
    }
}

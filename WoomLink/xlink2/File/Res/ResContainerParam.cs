using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResContainerParam
    {
#if XLINK_VER_THUNDER
        public byte TypeImpl;
        public byte Field1;
#elif XLINK_VER_BLITZ
        public ContainerType TypeImpl;
#endif
        public int ChildrenStartIndex;
        public int ChildrenEndIndex;
#if XLINK_VER_THUNDER
        public int Padding;
#endif

        public ContainerType Type => (ContainerType)TypeImpl;

        [StructLayout(LayoutKind.Sequential)]
        public struct ForSwitch
        {
#if XLINK_VER_THUNDER
            public Pointer<char> WatchPropertyName;
            public int Field8;
            public int FieldC;
            public int WatchPropertyId;
            public short LocalPropertyNameIdx;
            public short Field22;
            public byte Flags;
            public bool IsGlobal;
#elif XLINK_VER_BLITZ
            public Pointer<char> WatchPropertyName;
            public int WatchPropertyId;
            public short LocalPropertyNameIdx;
            public bool IsGlobal;
#endif
            /* WatchPropertyId? */
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ForMono
        {
            public Pointer<char> Unk1;
            public Pointer<char> Unk2;
        }

        public readonly bool IsRandom => Type is ContainerType.Random or ContainerType.Random2;
    }
}

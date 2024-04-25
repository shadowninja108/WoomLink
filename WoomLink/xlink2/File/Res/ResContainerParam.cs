using System.Runtime.InteropServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResContainerParam
    {
#if XLINK_VER_BLITZ || XLINK_VER_PARK
        public ContainerType TypeImpl;
#elif  XLINK_VER_THUNDER || XLINK_VER_EXKING
        public byte TypeImpl;
        /* TODO: what are these? */
        public byte Field1;
        public byte Field2;
#else
#error Invalid XLink version target.
#endif

        public int ChildrenStartIndex;
        public int ChildrenEndIndex;
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
        public int Padding; /* TODO: What is this? */
#endif

        public readonly ContainerType Type => (ContainerType)TypeImpl;

        [StructLayout(LayoutKind.Sequential)]
        public struct ForSwitch
        {
            public Pointer<char> WatchPropertyName;
            public int WatchPropertyId;
            public short LocalPropertyNameIdx;
            public bool IsGlobal;
        }

#if XLINK_VER_THUNDER || XLINK_VER_EXKING
        [StructLayout(LayoutKind.Sequential)]
        public struct ForGrid
        {
            public Pointer<char> Property1Name;
            public Pointer<char> Property2Name;
            public short Property1Index;
            public short Property2Index;
            public byte Flags;
            public byte Padding;
            public byte Property1ValueArrayCount;
            public byte Property2ValueArrayCount;

            public readonly bool IsProperty1Global => (Flags & 0b01) != 0;
            public readonly bool IsProperty2Global => (Flags & 0b10) != 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Special
        {
            public Pointer<char> Unk;
        }

#endif

        public readonly bool IsRandom => Type is ContainerType.Random or ContainerType.Random2;
    }
}

using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x48)]
    public struct ResourceHeader
    {
        public uint Magic;
        public uint DataSize;
        public uint Version;
        public uint NumResParam;
        public uint NumResAssetParam;
        public uint NumResTriggerOverwriteParam;
        public uint TriggerOverwriteParamTablePos;
        public uint LocalPropertyNameRefTablePos;
        public uint NumLocalPropertyNameRefTable;
        public uint NumLocalPropertyEnumNameRefTable;
        public uint NumDirectValueTable;
        public uint NumRandomTable;
        public uint NumCurveTable;
        public uint NumCurvePointTable;
        public uint ExRegionPos;
        public uint NumUser;
        public uint ConditionTablePos;
        public uint NameTablePos;
    }

}

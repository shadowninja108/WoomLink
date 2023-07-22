using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceHeader
    {
        public uint Magic;
        public uint DataSize;
        public uint Version;
        public int NumResParam;
        public int NumResAssetParam;
        public int NumResTriggerOverwriteParam;
        public UintPointer TriggerOverwriteParamTablePos;
        public UintPointer LocalPropertyNameRefTablePos;
        public int NumLocalPropertyNameRefTable;
        public int NumLocalPropertyEnumNameRefTable;
        public int NumDirectValueTable;
        public int NumRandomTable;
        public int NumCurveTable;
        public int NumCurvePointTable;
        public UintPointer ExRegionPos;
        public int NumUser;
        public UintPointer ConditionTablePos;
        public UintPointer NameTablePos;
    }
}

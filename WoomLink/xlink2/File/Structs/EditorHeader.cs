using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EditorHeader
    {
        public int Version;
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
        public UintPointer ExDataRegionPos;
        public UintPointer UserNamePos;
        public UintPointer UserBinPos;
        public UintPointer ConditionTablePos;
        public UintPointer NameTablePos;
    }
}

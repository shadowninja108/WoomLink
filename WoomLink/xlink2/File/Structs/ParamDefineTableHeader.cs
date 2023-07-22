using System.Runtime.InteropServices;

namespace WoomLink.xlink2.File.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ParamDefineTableHeader
    {
        public int Size;
        public int NumTotalUserParams;
        public int NumTotalAssetParams;
        public int NumUserAssetParams;
        public int NumTriggerParams;

        public int NumStandardAssetParams => NumTotalAssetParams - NumUserAssetParams;
    }
}

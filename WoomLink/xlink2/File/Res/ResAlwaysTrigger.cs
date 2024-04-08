using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ResAlwaysTrigger
    {
        public uint GuId;
        public ushort Flag;
        public ushort OverwriteHash;
        public Pointer<ResAssetCallTable> AssetCtb;
        public Pointer<ResTriggerOverwriteParam> OverwriteParamPos;
    }
}

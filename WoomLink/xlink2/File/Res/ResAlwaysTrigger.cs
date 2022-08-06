using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential, Size = 0x20)]
    public struct ResAlwaysTrigger
    {
        public uint GuId;
        public uint AssetCtb;
        public ushort Flag;
        public ushort OverwriteHash;
        public uint OverwriteParamPos;
    }
}

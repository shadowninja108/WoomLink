using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential, Size = 0x14)]
    public struct ResCurveCallTable
    {
        public ushort CurvePointStartPos;
        public ushort NumPoint;
        public ushort CurveType;
        public ushort IsPropGlobal;
        public uint PropName;
        public uint PropIdx;
        public short LocalPropertyNameIdx;
        public ushort Padding;

        public bool IsPropGlobalBool => IsPropGlobal != 0;
    }
}

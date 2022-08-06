using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Sequential, Size = 0xC)]
    public struct ResAction
    {
        public uint Name;
        public ushort TriggerStartIdx;
        public ushort TriggerEndIdx;
    }
}

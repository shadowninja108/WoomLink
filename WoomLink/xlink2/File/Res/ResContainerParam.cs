using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential, Size = 0xC)]
    public struct ResContainerParam
    {
        public ContainerType Type;
        public uint ChildrenStartIndex;
        public uint ChildrenEndIndex;
    }
}

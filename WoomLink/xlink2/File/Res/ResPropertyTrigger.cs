using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{
    public struct ResPropertyTrigger
    {
        public uint GuId;
        public uint AssetCtbPos;
        public int Condition;
        public ushort Flag;
        public ushort OverwriteHash;
        public int OverwritePos;
    }
}

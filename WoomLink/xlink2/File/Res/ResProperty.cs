using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{
    public struct ResProperty
    {
        public uint WatchPropertyNamePos;
        public uint IsGlobal;
        public uint TriggerStartIdx;
        public uint TriggerEndIdx;

        public bool IsGlobalBool => IsGlobal != 0;
    }
}

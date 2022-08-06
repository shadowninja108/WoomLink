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
    [StructLayout(LayoutKind.Sequential, Size = 0x18)]
    public struct ResActionTrigger
    {
        public uint GuId;
        public uint AssetCtbPos;
        public uint StartFrame;
        public int EndFrame;
        public ushort Flag;
        public ushort OverwriteHash;
        public int OverwriteParamPos;

        public ActionTriggerType TriggerType
        {
            get
            {
                var flag = Flag >> 2;
                if ((flag & (1 << 2)) != 0)
                    return ActionTriggerType.Three;
                if ((flag & (1 << 1)) != 0)
                    return ActionTriggerType.Two;
                if ((flag & (1 << 0)) != 0)
                    return ActionTriggerType.One;
                return ActionTriggerType.Zero;
            }
        }
    }
}

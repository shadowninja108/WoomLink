using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential, Size = 0x30)]
    public struct ResUserHeader
    {
        public uint IsSetup;
        public uint NumLocalProperty;
        public uint NumCallTable;
        public uint NumAsset;
        public uint NumRandomContainer;
        public uint NumResActionSlot;
        public uint NumResAction;
        public uint NumResActionTrigger;
        public uint NumResProperty;
        public uint NumResPropertyTrigger;
        public uint NumResAlwaysTrigger;
        public uint TriggerTablePos;
    }
}

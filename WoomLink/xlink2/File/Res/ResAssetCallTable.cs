using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res
{

    [StructLayout(LayoutKind.Sequential, Size = 0x20, Pack = 1)]
    public struct ResAssetCallTable
    {
        public uint KeyNamePos;
        public ushort AssetId;
        public ushort Flag;
        public int Duration;
        public uint ParentIndex;
        public ushort EnumIndex;
        public byte IsSolved;
        public byte Unk13;
        public uint KeyNameHash;
        public int ParamStartPos;
        public int ConditionPos;

        public bool IsContainer => (Flag & 1) != 0;
        public bool IsSolvedBool => IsSolved != 0;
    }
}

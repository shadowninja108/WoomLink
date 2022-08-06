using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res.Ex;

namespace WoomLink.xlink2.File.Res
{
    public class ResTriggerOverwriteParam
    {

        public uint Bitfield;
        public ResParamEx[] Params;
        private readonly uint[] _rawValues;

        private readonly uint TriggerNum;

        public ResTriggerOverwriteParam(Stream stream, uint triggerNum)
        {
            TriggerNum = triggerNum;
            BinaryReader binaryReader = new(stream);

            Bitfield = binaryReader.ReadUInt32();
            _rawValues = stream.ReadArray<uint>((uint)BitOperations.PopCount(Bitfield));
        }

        public bool IsParamDefault(int bit)
        {
            return (Bitfield & 1UL << bit) == 0;
        }

        /* Normally done at access time. */
        public void Solve(Stream stream, CommonResourceParam param, ParamDefineTable pdt)
        {
            Params = new ResParamEx[TriggerNum];
            var valueIdx = 0;
            for (var bitIdx = 0; bitIdx < TriggerNum; bitIdx++)
            {
                if (IsParamDefault(bitIdx))
                    continue;

                var v = new ResParamEx(_rawValues[valueIdx]);
                // v.Solve(stream, param, pdt.TriggerParam[bitIdx]);
                Params[bitIdx] = v;
                valueIdx++;
            }
        }

        public static IEnumerable<ResTriggerOverwriteParam> Read(Stream stream, uint resTriggerOverwriteParamNum, uint triggerNum)
        {
            for (var i = 0; i < resTriggerOverwriteParamNum; i++)
            {
                yield return new ResTriggerOverwriteParam(stream, triggerNum);
            }
        }
    }
}

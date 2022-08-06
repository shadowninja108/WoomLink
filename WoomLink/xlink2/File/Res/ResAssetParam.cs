using System.Collections.Generic;
using System.IO;
using System.Numerics;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res.Ex;

namespace WoomLink.xlink2.File.Res
{
    public class ResAssetParam
    {
        public ulong Bitfield;
        public ResParamEx[] Params;
        private readonly uint[] _rawValues;

        private readonly uint AssetNum;

        public ResAssetParam(Stream stream, uint assetNum)
        {
            BinaryReader binaryReader = new(stream);
            AssetNum = assetNum;

            Bitfield = binaryReader.ReadUInt64();
            _rawValues = stream.ReadArray<uint>((uint)BitOperations.PopCount(Bitfield));
        }

        public bool IsParamDefault(int bit)
        {
            return (Bitfield & 1UL << bit) == 0;
        }

        /* Normally handled at access time. */
        public void Solve(Stream stream, CommonResourceParam param, ParamDefineTable pdt)
        {
            Params = new ResParamEx[AssetNum];
            var valueIdx = 0;
            for (var bitIdx = 0; bitIdx < AssetNum; bitIdx++)
            {
                if (IsParamDefault(bitIdx))
                    continue;
                
                var v = new ResParamEx(_rawValues[valueIdx]);
                v.Solve(stream, param, pdt.AssetParam[bitIdx]);
                Params[bitIdx] = v;
                valueIdx++;
            }
        }

        public static IEnumerable<ResAssetParam> Read(Stream stream, uint assetParamNum, uint assetNum)
        {
            for (var i = 0; i < assetParamNum; i++)
            {
                yield return new ResAssetParam(stream, assetNum);
            }
        }
    }
}

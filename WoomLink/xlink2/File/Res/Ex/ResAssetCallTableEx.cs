using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoomLink.sead;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResAssetCallTableEx
    {
        public ResAssetCallTable Internal;
        public string KeyName;

        public ResContainerParamEx ContainerParam;
        public ResAssetParam AssetParam;
        public ResCondition Condition;

        public ResAssetCallTableEx(ResAssetCallTable @internal)
        {
            Internal = @internal;
        }

        public void Solve(Stream stream, CommonResourceParam commonParam, UserBinParam userParam, ParamDefineTable pdt)
        {
            using (stream.TemporarySeek(commonParam.NameTablePos + Internal.KeyNamePos, SeekOrigin.Begin))
                KeyName = new BinaryReader(stream).ReadUtf8Z();

            /* Yes, they calculate this at runtime... */
            Internal.KeyNameHash = HashCrc32.CalcStringHash(KeyName);

            if (Internal.IsContainer)
            {
                if (Internal.ParamStartPos == -1)
                {
                    Internal.ParamStartPos = 0;
                }
                else
                {
                    using (stream.TemporarySeek(userParam.ContainerParamTablePos + Internal.ParamStartPos, SeekOrigin.Begin))
                    {
                        ContainerParam = ResContainerParamEx.Read(stream, 1).First();
                        ContainerParam.Solve(stream, commonParam);
                    }
                }
            }
            else
            {
                using (stream.TemporarySeek(commonParam.AssetParamsPos + Internal.ParamStartPos, SeekOrigin.Begin))
                {
                    AssetParam = ResAssetParam.Read(stream, 1, pdt.Header.NumAssetParams).First();
                    /* Not normally done. */
                    AssetParam.Solve(stream, commonParam, pdt);
                }
            }

            if (Internal.ConditionPos == -1)
            {
                Internal.ConditionPos = 0;
            }
            else
            {
                using (stream.TemporarySeek(commonParam.ConditionTablePos + Internal.ConditionPos, SeekOrigin.Begin))
                {
                    Condition = ResCondition.Read(stream, uint.MaxValue).First();
                    Condition.Solve(stream, commonParam);
                }
            }
        }

        public static IEnumerable<ResAssetCallTableEx> Read(Stream stream, uint num)
        {
            for (int i = 0; i < num; i++)
            {
                var assetCtb = new ResAssetCallTable();
                stream.Read(Utils.AsSpan(ref assetCtb));
                yield return new ResAssetCallTableEx(assetCtb);
            }
        }
    }
}

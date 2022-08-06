using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResPropertyTriggerEx
    {
        public ResPropertyTrigger Internal;

        public ResAssetCallTableEx AssetCtb;
        public ResCondition Condition;
        public ResTriggerOverwriteParam TriggerOverwriteParam;

        public ResPropertyTriggerEx(ResPropertyTrigger @internal)
        {
            Internal = @internal;
        }

        public void Solve(Stream stream, CommonResourceParam param, UserBinParam userParam, ParamDefineTable pdt)
        {
            using (stream.TemporarySeek(userParam.AssetCallTablePos + Internal.AssetCtbPos, SeekOrigin.Begin))
            {
                AssetCtb = ResAssetCallTableEx.Read(stream, 1).First();
                AssetCtb.Solve(stream, param, userParam, pdt);
            }

            if (Internal.Condition != -1)
            {
                using (stream.TemporarySeek(param.ConditionTablePos + Internal.Condition, SeekOrigin.Begin))
                {
                    Condition = ResCondition.Read(stream, uint.MaxValue).First();
                    Condition.Solve(stream, param);
                }
            }

            if (Internal.OverwritePos != -1)
            {
                using (stream.TemporarySeek(param.TriggerOverwriteParamsPos + Internal.OverwritePos, SeekOrigin.Begin))
                {
                    TriggerOverwriteParam =
                        ResTriggerOverwriteParam.Read(stream, 1, pdt.Header.NumTriggerParams).First();
                    /* Normally done at access time. */
                    TriggerOverwriteParam.Solve(stream, param, pdt);
                }
            } 
        }
    }
}

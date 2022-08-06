using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResActionTriggerEx
    {
        public ResActionTrigger Internal;
        public string StartFrameAsString; /* ? */
        public ResAssetCallTableEx AssetCtb;
        public ResTriggerOverwriteParam OverwriteParam;

        public ResActionTriggerEx(ResActionTrigger @internal)
        {
            Internal = @internal;
        }
        
        public void Solve(Stream stream, CommonResourceParam param, UserBinParam userParam, ParamDefineTable pdt)
        {

            using (stream.TemporarySeek(userParam.AssetCallTablePos + Internal.AssetCtbPos, SeekOrigin.Begin))
            {
                ResAssetCallTable assetCtb = new();
                stream.Read(Utils.AsSpan(ref assetCtb));
                AssetCtb = new ResAssetCallTableEx(assetCtb);
                AssetCtb.Solve(stream, param, userParam, pdt);
            }

            if (Internal.OverwriteParamPos == -1)
            {
                using (stream.TemporarySeek(param.TriggerOverwriteParamsPos + Internal.OverwriteParamPos, SeekOrigin.Begin))
                {
                    OverwriteParam = ResTriggerOverwriteParam.Read(stream, 1, pdt.Header.NumTriggerParams).First();
                    OverwriteParam.Solve(stream, param, pdt);
                }
            }

            if (Internal.TriggerType == ActionTriggerType.Three)
            {
                using (stream.TemporarySeek(param.NameTablePos + Internal.StartFrame, SeekOrigin.Begin))
                    StartFrameAsString = new BinaryReader(stream).ReadUtf8Z();
            }
        }
    }
}

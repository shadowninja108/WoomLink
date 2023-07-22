using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public class ResourceAccessorSLink : ResourceAccessor
    {
        public ResourceAccessorSLink(UserResource resource, System system) : base(resource, system)
        {
        }

        public override bool IsBlankAsset(ref ResAssetCallTable table)
        {
            if (CheckAndErrorIsAsset(ref table, "isBlankAsset"))
                return false;


            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, 1);
            string result;
            if (!param.IsNull())
            {
                result = GetResParamValueString(ref param.Ref);
            }
            else
            {
                ref var pdt = ref System.GetParamDefineTable();
                result = pdt.GetAssetParamDefaultValueString(1);
            }

            return result == "@Blank";
        }

        public override string GetBoneName(ref ResAssetCallTable table)
        {
            if (CheckAndErrorIsAsset(ref table, "getBoneName"))
                return "";

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, 15);
            if (!param.IsNull())
            {
                return GetResParamValueString(ref param.Ref);
            }
            else
            {
                ref var pdt = ref System.GetParamDefineTable();
                return pdt.GetAssetParamDefaultValueString(15);
            }
        }

        public override bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param)
        {
            return IsParamOverwritten(param, 15);
        }

        public override string GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param)
        {
            return GetResOverwriteParamValueString(param, 15);
        }

        public string GetBoneNameWithOverwrite(ref ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> param)
        {
            if (IsParamOverwritten(param, 15))
                return GetOverwriteBoneName(param);
            else
                return GetBoneName(ref table);
        }

        public override bool IsAutoOneTimeFade(ref ResAssetCallTable table)
        {
            return ((GetAssetBitFlag(ref table) >> 5) & 1) != 0;
        }

        public override bool IsForceLoopAsset(ref ResAssetCallTable table)
        {
            return ((GetAssetBitFlag(ref table) >> 6) & 1) != 0;
        }

        public override float GetDelayWithOverwrite(ref ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance)
        {
            float ret;
            if (IsParamOverwritten(overwriteParam, 11))
                ret = GetResOverwriteParamValueFloat(overwriteParam, 11, instance);
            else
            {
                if (!CheckAndErrorIsAsset(ref table, "getDelay"))
                    return 0;

                var param = GetResParamFromAssetParamPos(table.ParamAsAsset, 11);
                if (!param.IsNull())
                    ret = GetResParamValueFloat(ref param.Ref, instance);
                else
                    ret = System.GetParamDefineTable().GetAssetParamDefaultValueFloat(11);
            }

            return float.Max(ret, 0f);
        }

        public override float GetDuration(ref ResAssetCallTable table, UserInstance instance)
        {
            if (!CheckAndErrorIsAsset(ref table, "getDuration"))
                return 0;

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, 12);
            float ret;
            if (!param.IsNull())
                ret = GetResParamValueFloat(ref param.Ref, instance);
            else
                ret = System.GetParamDefineTable().GetAssetParamDefaultValueFloat(12);

            return float.Max(ret, 0f);
        }

        private static readonly int[] TriggerOverwriteParamIdLookup =
        {
            0, -1, -1, 1, 2, 3, 4, -1, 5, -1, 6, -1, 7
        };

        protected override int GetTriggerOverwriteParamId(uint reff)
        {
            if((reff - 3) > 0xC)
                return 1;
            return TriggerOverwriteParamIdLookup[reff];
        }

        protected override int GetAssetBitFlag(ref ResAssetCallTable table)
        {
            if (CheckAndErrorIsAsset(ref table, "getAssetBitFlag_"))
                return 0;

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, 17);
            if (!param.IsNull())
            {
                return GetResParamValueInt(ref param.Ref);
            }
            else
            {
                return System.GetParamDefineTable().GetAssetParamDefaultValueInt(17);
            }
        }
    }
}

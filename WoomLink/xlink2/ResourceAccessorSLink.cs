using WoomLink.Ex;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.User.Instance;
using WoomLink.xlink2.User.Resource;

namespace WoomLink.xlink2
{
    public class ResourceAccessorSLink : ResourceAccessor
    {
        /* TODO: these have probably changed between versions... */
        private const uint AssetNameIndex = 1;
        private const uint DelayIndex = 11;
        private const uint DurationIndex = 12;
        private const uint BoneNameIndex = 15;
        private const uint AssetBitFlagIndex = 17;
        
        private const uint IsAutoOneTimeFadeMask = 1 << 5;
        private const uint IsForceLoopAssetMask = 1 << 6;

        public ResourceAccessorSLink(UserResource resource, System system) : base(resource, system)
        {
        }

        public Pointer<char> GetAssetName(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "getAssetName", AssetNameIndex);

        public override bool IsBlankAsset(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "isBlankAsset", AssetNameIndex).AsString() == "@Blank";

        public override Pointer<char> GetBoneName(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "getBoneName", BoneNameIndex);

        public override bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param) => IsParamOverwritten(param, BoneNameIndex);

        public override Pointer<char> GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param) => GetResOverwriteParamValueString(param, BoneNameIndex);

        public Pointer<char> GetBoneNameWithOverwrite(in ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> param)
        {
            if (IsParamOverwritten(param, BoneNameIndex))
                return GetOverwriteBoneName(param);
            else
                return GetBoneName(in table);
        }

        public override bool IsAutoOneTimeFade(in ResAssetCallTable table) => (GetAssetBitFlag(in table) & IsAutoOneTimeFadeMask) != 0;

        public override bool IsForceLoopAsset(in ResAssetCallTable table) => (GetAssetBitFlag(in table) & IsForceLoopAssetMask) != 0;

        public override float GetDelayWithOverwrite(in ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance)
        {
            float ret;
            if (IsParamOverwritten(overwriteParam, 11))
                ret = GetResOverwriteParamValueFloat(overwriteParam, 11, instance);
            else
                ret = GetFloatParamFromAsset(in table, "getDelay", DelayIndex, instance);
            return float.Max(ret, 0f);
        }

        public override float GetDuration(in ResAssetCallTable table, UserInstance instance) => float.Max(GetFloatParamFromAsset(in table, "getDuration", DurationIndex, instance), 0f);

        private static readonly int[] TriggerOverwriteParamIdLookup =
        [
            0, -1, -1, 1, 2, 3, 4, -1, 5, -1, 6, -1, 7
        ];

        protected override int GetTriggerOverwriteParamId(uint reff)
        {
            var index = reff - 3;
            if(index > TriggerOverwriteParamIdLookup.Length)
                return -1;
            return TriggerOverwriteParamIdLookup[index];
        }

        protected override uint GetAssetBitFlag(in ResAssetCallTable table) => (uint) GetIntParamFromAsset(in table, "getAssetBitFlag_", AssetBitFlagIndex);
    }
}

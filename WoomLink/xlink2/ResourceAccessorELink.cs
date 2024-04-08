using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.User.Instance;
using WoomLink.xlink2.User.Resource;

namespace WoomLink.xlink2
{
    public class ResourceAccessorELink : ResourceAccessor
    {
        /* TODO: these have probably changed between versions... */
        public static uint AssetNameIndex           = 1;
        public static uint GroupNameIndex           = 3;
        public static uint DelayIndex               = 5;
        public static uint DurationIndex            = 6;
        public static uint ClipTypeIndex            = 7;
        public static uint BoneNameIndex            = 11;
        public static uint AssetBitFlagIndex        = 28;

        public static uint IsAutoOneTimeFadeMask    = 1 << 0;
        public static uint IsNeedObserveMask        = 1 << 3;
        public static uint UnkMask                  = 1 << 4;
        public static uint IsForceLoopAssetMask     = 1 << 6;

        public int Unk28 = 0;

        public ResourceAccessorELink(UserResource resource, System system) : base(resource, system)
        {
        }

        public Pointer<char> GetAssetName(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "getAssetName", AssetNameIndex);

        public ResEset GetEsetVal(in ResAssetCallTable table)
        {
            var pair = ((UserResourceELink)Resource).GetSolvedAssetParameterELink(in table);
            if (!pair.IsNull)
                return pair.Ref.EsetVal;
            else
                return new ResEset();
        }

        public Pointer<char> GetGroupName(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "getGroupName", GroupNameIndex);

        public int GetGroupId(in ResAssetCallTable table)
        {
            var pair = ((UserResourceELink)Resource).GetSolvedAssetParameterELink(in table);
            if (!pair.IsNull)
                return pair.Ref.GroupId;
            else
                return -1;
        }

        public ClipType GetClipType(in ResAssetCallTable table)
        {
            var value = (ClipType) GetIntParamFromAsset(table, "getClipType", ClipTypeIndex);

            if (value == ClipType.None)
            {
                if (IsNeedObserve(in table))
                {
                    return ClipType.Unk1;
                }
                else
                {
                    return ClipType.Kill;
                }
            }

            return value;
        }

        public override bool IsBlankAsset(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "isBlankAsset", AssetNameIndex).AsString() == "@Blank";

        public override Pointer<char> GetBoneName(in ResAssetCallTable table) => GetStringParamFromAsset(in table, "getBoneName", BoneNameIndex);

        public override bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param) => IsParamOverwritten(param, BoneNameIndex);

        public override Pointer<char> GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param) => GetResOverwriteParamValueString(param, BoneNameIndex);

        public override bool IsAutoOneTimeFade(in ResAssetCallTable table) => (GetAssetBitFlag(in table) & IsAutoOneTimeFadeMask) != 0;
        public override bool IsForceLoopAsset(in ResAssetCallTable table) => (GetAssetBitFlag(in table) & IsForceLoopAssetMask) != 0;
        
        public override float GetDelayWithOverwrite(in ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance)
        {
            float value;
            if (IsParamOverwritten(overwriteParam, DelayIndex))
            {
                value = GetResOverwriteParamValueFloat(overwriteParam, DelayIndex, instance);
            }
            else
            {
                value = GetFloatParamFromAsset(in table, "getDelay", DelayIndex, instance);
            }

            return float.Max(value, 0);
        }

        public override float GetDuration(in ResAssetCallTable table, UserInstance instance) => GetFloatParamFromAsset(in table, "getDuration", DurationIndex, instance);

        private static readonly int[] TriggerOverwriteParamIdLookup =
        [
            0, -1, -1, -1, -1, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 0
        ];

        protected override int GetTriggerOverwriteParamId(uint reff)
        {
            var index = reff - 5;
            if (index > TriggerOverwriteParamIdLookup.Length)
                return -1;
            return TriggerOverwriteParamIdLookup[index];
        }

        protected override uint GetAssetBitFlag(in ResAssetCallTable table) => (uint) GetIntParamFromAsset(in table, "getAssetBitFlag_", AssetBitFlagIndex);
    }
}

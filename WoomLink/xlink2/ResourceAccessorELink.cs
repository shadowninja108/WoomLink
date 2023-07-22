using System;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public class ResourceAccessorELink : ResourceAccessor
    {
        public int Unk28 = 0;

        public ResourceAccessorELink(UserResource resource, System system) : base(resource, system)
        {
        }

        public override bool IsBlankAsset(ref ResAssetCallTable table)
        {
            throw new NotImplementedException();
        }

        public override string GetBoneName(ref ResAssetCallTable table)
        {
            throw new NotImplementedException();
        }

        public override bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param)
        {
            throw new NotImplementedException();
        }

        public override string GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param)
        {
            throw new NotImplementedException();
        }

        public override bool IsAutoOneTimeFade(ref ResAssetCallTable table)
        {
            throw new NotImplementedException();
        }

        public override bool IsForceLoopAsset(ref ResAssetCallTable table)
        {
            throw new NotImplementedException();
        }

        public override float GetDelayWithOverwrite(ref ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance)
        {
            throw new NotImplementedException();
        }

        public override float GetDuration(ref ResAssetCallTable table, UserInstance instance)
        {
            throw new NotImplementedException();
        }

        protected override int GetTriggerOverwriteParamId(uint reff)
        {
            throw new NotImplementedException();
        }

        protected override int GetAssetBitFlag(ref ResAssetCallTable table)
        {
            throw new NotImplementedException();
        }
    }
}

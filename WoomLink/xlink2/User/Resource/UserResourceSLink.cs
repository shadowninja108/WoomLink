using System;
using WoomLink.xlink2.File;

namespace WoomLink.xlink2
{
    public class UserResourceSLink : UserResource
    {
        public ResourceAccessorSLink Accessor;
        public UserResourceSLink(User user) : base(user)
        {
            Accessor = new ResourceAccessorSLink(this, SystemSLink.Instance);
        }

        public override ResourceAccessor GetAccessor() => Accessor;

        public override ResourceAccessor GetAccessorPtr() => Accessor;
        public override System GetSystem() => SystemSLink.Instance;

        public override UserResourceParam AllocResourceParam()
        {
            throw new NotImplementedException();
        }

        public override void FreeResourceParam(UserResourceParam param)
        {
            throw new NotImplementedException();
        }

        public override void OnSetupResourceParam_(UserResourceParam param, in ParamDefineTable table)
        {
            var paramSLink = (UserResourceParamSLink) param;

            if (param.Common.Value.NameTable == 0)
                return;

            if (!param.User.UserParamArry.IsNull())
            {
                /* Populate parameters from the ResParam. */
            }

            /* If either aal::System has a loop asset list reader or param.User.GetLeaderInstance().AssetInfoReader exists and has more than one value, call SolveIsLoop_ and SolveNeedObserveFlag_. */
        }

        public void SolveIsLoop(UserResourceParam param, ref ParamDefineTable table /* aal::IAssetInfoReadable*/)
        {
            throw new NotImplementedException();
        }
    }
}

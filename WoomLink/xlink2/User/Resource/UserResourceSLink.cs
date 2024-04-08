using System;
using System.Diagnostics;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.User.Resource
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
        public override SystemSLink GetSystem() => SystemSLink.Instance;

        public override UserResourceParam AllocResourceParam()
        {
            var param = new UserResourceParamSLink
            {
                Accessor = Accessor
            };
            return param;
        }

        public override void FreeResourceParam(UserResourceParam param)
        {
            throw new NotImplementedException();
        }

        public override void OnSetupResourceParam_(UserResourceParam param, in ParamDefineTable table)
        {
            var paramSLink = (UserResourceParamSLink) param;

            if (param.Common.NameTablePointer == 0)
                return;

            if (!param.User.UserParamArry.IsNull)
            {
                var array = param.User.UserParamArry.AsSpan(8);

                var defs = param.Accessor.System.ResourceBuffer.PDT.UserParamSpan;

                Debug.Assert(array[0].ReferenceType == ValueReferenceType.String);
                Debug.Assert(defs[0].Type           == ParamType.String);

                Debug.Assert(array[1].ReferenceType == ValueReferenceType.String);
                Debug.Assert(defs[1].Type           == ParamType.String);

                Debug.Assert(array[2].ReferenceType == ValueReferenceType.Direct);
                Debug.Assert(defs[2].Type           == ParamType.Enum);

                Debug.Assert(array[3].ReferenceType == ValueReferenceType.Direct);
                Debug.Assert(defs[3].Type           == ParamType.Int);

                Debug.Assert(array[4].ReferenceType == ValueReferenceType.Direct);
                Debug.Assert(defs[4].Type           == ParamType.Float);

                Debug.Assert(array[5].ReferenceType == ValueReferenceType.Direct);
                Debug.Assert(defs[5].Type           == ParamType.Float);

                /* TODO: what should the type be here? */
                Debug.Assert(array[6].ReferenceType == ValueReferenceType.Direct);
                Debug.Assert(defs[6].Type           == ParamType.Int);

                Debug.Assert(array[7].ReferenceType == ValueReferenceType.Bitfield);
                Debug.Assert(defs[7].Type           == ParamType.Bitfield);

                paramSLink.GroupName            = array[0].GetAsString(in param.Common);
                paramSLink.DistanceParamSetName = array[1].GetAsString(in param.Common);
                paramSLink.LimitType            = array[2].GetAsInt(in param.Common);
                paramSLink.PlayableLimitNum     = array[3].GetAsInt(in param.Common);
                paramSLink.Priority             = array[4].GetAsFloat(in param.Common);
                paramSLink.NotPositioned        = (array[7].GetAsInt(in param.Common) & 1) == 1;
                paramSLink.DopplerFactor        = array[5].GetAsFloat(in param.Common);

                var arrangeGroupParamsStart = param.Common.ExRegionPointer.AddBytes(array[6].Value).Cast<int>();
                paramSLink.ArrangeGroupParamsCount = arrangeGroupParamsStart.Value;
                paramSLink.ArrangeGroupParamsPointer = arrangeGroupParamsStart.AtEnd<ArrangeGroupParam>();
            }

            /* If either aal::System has a loop asset list reader or param.User.GetLeaderInstance().AssetInfoReader exists and has more than one value, call SolveIsLoop_ and SolveNeedObserveFlag_. */
        }

        public void SolveIsLoop(UserResourceParam param, ref ParamDefineTable table /* aal::IAssetInfoReadable*/)
        {
            throw new NotImplementedException();
        }
    }
}

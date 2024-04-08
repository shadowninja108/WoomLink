using System;
using WoomLink.xlink2.File;

namespace WoomLink.xlink2.User.Instance
{
    public class UserInstanceSLink : UserInstance
    {
        public class CreateArgSLink : CreateArg
        {
            public byte Unk40;
            /* heap for allocating aal arbiter */
        }

        public UserInstanceSLink(CreateArgSLink arg, System system, User user) : base(arg, system, user)
        {
            /* Setup state and various other aal stuff we don't care about. */
        }

        public override void OnPostCalc()
        {
            throw new NotImplementedException();
        }

        public override UserInstanceParam AllocInstanceParam()
        {
            return new UserInstanceParam();
        }

        public override void FreeInstanceParam(UserInstanceParam param, ResMode mode)
        {
            throw new NotImplementedException();
        }

        public override void OnSetupInstanceParam(ResMode mode)
        {
            /* Setting up asset limiters and executors. */
            // throw new NotImplementedException();
        }

        public override void InitModelAssetConnection(ResMode mode, in ParamDefineTable paramDefine)
        {
            throw new NotImplementedException();
        }

        public override bool DoEventActivatingCallback(Locator locator)
        {
            throw new NotImplementedException();
        }

        public override void DoEventActivatedCallback(Locator locator, Event e)
        {
            throw new NotImplementedException();
        }
    }
}

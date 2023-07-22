using System;
using WoomLink.xlink2.File;

namespace WoomLink.xlink2
{
    public class UserInstanceELink : UserInstance
    {
        private byte DefaultGroup;
        private int FieldF8 = -1;
        public UserInstanceELink(CreateArg arg, System system, User user /* heap */) : base(arg, system, user)
        {
        }

        public override int GetDefaultGroup()
        {
            return DefaultGroup;
        }

        public override void OnPostCalc()
        {
            foreach (var even in Events)
            {
                if(!even.Calc())
                    continue;

                User.GetSystem().FreeEvent(even, Events);
            }
        }

        public override void OnReset()
        {
            /* TODO: state */
            FieldF8 = -1;
        }

        public override UserInstanceParam AllocInstanceParam()
        {
            return new UserInstanceParam();
        }

        public override void FreeInstanceParam(UserInstanceParam param, ResMode mode)
        {
            FreeInstanceParam(param);
        }

        public override void OnSetupInstanceParam(ResMode mode)
        {
            InitModelAssetConnection(mode, ref User.GetSystem().GetParamDefineTable());
        }

        public override void InitModelAssetConnection(ResMode mode, ref ParamDefineTable paramDefine)
        {
            throw new NotImplementedException();
        }

        public override void DoEventActivatedCallback(Locator locator, Event e)
        {
            throw new NotImplementedException();
        }
    }
}

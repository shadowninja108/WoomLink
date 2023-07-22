using System;
using System.Collections.Generic;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2
{
    public abstract class UserInstance
    {
        public class CreateArg
        {
            public string Name;
            /* IUser* User */
            /* Matrix34* Mtx */
            /* Bunch of unk bytes... */
            /* Vector3f* RootPos */
            /* Vector3f Scale */
            /* int ActionSlotCount */
            public uint LocalPropertyCount;
            public string[] ActionSlotNames;
        }

        public List<Event> Events = new();
        public UserInstanceParam[] Params = new UserInstanceParam[(int)ResMode.Count];
        public User User;
        /* IUser */
        /* Matrix34* RootMtx */
        /* Bunch of unk bytes copied from CreateArg... */
        /* Vector3f* RootPos */
        /* Vector3f* Scale */
        public float Unk60 = float.PositiveInfinity;
        public ulong ValueChangedBitfield = 0;
        public uint[] PropertyValues;
        /* TriggerCtrlMgr */
        /* Bunch of unk... */
        /* State */
        /* Unk pointer */
        /* Buffer of unk */

        public UserInstance(
            CreateArg arg,
            System system,
            User user
            /* heap */
        )
        {
            User = user;

            /* Generally import all of the arg. */
            /* Init TriggerCtrlMgr */
            /* Assign RootMtx to ident if null */
            /* Assign Scale to ones if null */

            if (User.PropertyDefinitionTable.Length > 0)
                PropertyValues = new uint[User.PropertyDefinitionTable.Length];
        }

        public void SetupResource( /* heap */)
        {
            if (!User.GetSystem().CallEnable)
                return;

            var system = User.GetSystem();
            var loc = system.GetModuleLockObj();
            loc.Lock();
            var instanceParam = Params[(int)ResMode.Normal];
            if (IsSetupRomInstanceParam())
            {
                var resource = User.UserResource;
                var resourceParam = resource.Params[(int)ResMode.Normal];
                if (resourceParam != null && !resourceParam.Setup)
                {
                    resource.Setup();
                }

                if (instanceParam == null)
                {
                    Params[(int)ResMode.Normal] = AllocInstanceParam();
                    SetupInstanceParam(ResMode.Normal);
                    /* TODO: TriggerCtrlMgr::AllocAndSetupCtrlParam */
                }

                User.GetSystem().RegisterUserForGlobalPropertyTrigger(User);
            }

            if (User.UserResource.ResMode == ResMode.Editor)
            {
                /* TODO: editor */
                throw new Exception();
            }
            loc.Unlock();


            for (var i = 0; i < User.PropertyDefinitionTable.Length; i++)
            {
                if((byte)User.PropertyDefinitionTable[i].Type >= (byte) PropertyType.End)
                    return;

                PropertyValues[i] = 0;
            }
        }

        private void SetupInstanceParam(ResMode mode /* heap */)
        {
            var instanceParam = Params[(int)mode];
            var resourceParam = User.UserResource.Params[(int)mode];

            var assetNum = resourceParam.User.Header.NumAsset;
            if (assetNum > 0)
            {
                instanceParam.Connections = new ModelAssetConnection[assetNum];
                for (var i = 0; i < assetNum; i++)
                {
                    instanceParam.Connections[i] = new ModelAssetConnection();
                }
            }

            var randomNum = resourceParam.User.Header.NumRandomContainer;
            if (randomNum > 0)
            {
                instanceParam.RandomHistory = new UserInstanceParam.RandomEvent[randomNum];
                for (var i = 0; i < randomNum; i++)
                {
                    instanceParam.RandomHistory[i] = new UserInstanceParam.RandomEvent();
                }
            }

            OnSetupInstanceParam(mode);
            instanceParam.Setup = true;
        }

        private bool IsSetupRomInstanceParam()
        {
            var param = Params[(int)ResMode.Normal];
            return param != null && !param.Setup;
        }

        protected void FreeInstanceParam(UserInstanceParam param)
        {
            /* Shout out to the garbage collector. */
        }

        /* makeDebugStringEvent */
        public virtual int GetDefaultGroup() => 0;
        public abstract void OnPostCalc();
        public abstract void OnReset();
        public abstract UserInstanceParam AllocInstanceParam( /* heap */);
        public abstract void FreeInstanceParam(UserInstanceParam param, ResMode mode);
        public abstract void OnSetupInstanceParam(ResMode mode /* heap */);
        public abstract void InitModelAssetConnection(ResMode mode, ref ParamDefineTable paramDefine /* heap */);

        public virtual bool DoEventActivatingCallback(Locator locator) => false;

        public virtual void DoEventActivatedCallback(Locator locator, Event e) { }
    }
}

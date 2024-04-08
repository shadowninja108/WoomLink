using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public static class ResourceUtil
    {
        public static Pointer<ResContainerParam> GetResSwitchContainerParam(in ResAssetCallTable act)
        {
            if(!act.IsContainer)
                return Pointer<ResContainerParam>.Null;

            var containerPtr = act.ParamAsContainer;
            if(containerPtr.IsNull)
                return Pointer<ResContainerParam>.Null;
            
            if(containerPtr.Ref.Type != ContainerType.Switch)
                return Pointer<ResContainerParam>.Null;

            return containerPtr;
        }

        public static ActionTriggerType GetActionTriggerType(in ResActionTrigger trigger)
        {
                var flag = trigger.Flag >> 2;
                if ((flag & (1 << 2)) != 0)
                    return ActionTriggerType.Three;
                if ((flag & (1 << 1)) != 0)
                    return ActionTriggerType.Two;
                if ((flag & (1 << 0)) != 0)
                    return ActionTriggerType.One;
                return ActionTriggerType.Zero;
        }
    }
}

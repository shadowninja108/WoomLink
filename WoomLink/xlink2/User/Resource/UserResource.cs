using System;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.xlink2
{
    public abstract class UserResource
    {
        public User User;
        public ResMode ResMode;
        public UserResourceParam[] Params;

        public UserResourceParam NormalParam
        {
            get => Params[(int)ResMode.Normal];
            set => Params[(int)ResMode.Normal] = value;
        }

        public UserResourceParam CurrentParam
        {
            get => Params[(int)ResMode];
            set => Params[(int)ResMode] = value;
        }

        protected UserResource(User user)
        {
            User = user;
            ResMode = ResMode.Normal;
            Params = new UserResourceParam[(int)ResMode.Count];
        }

        public void Setup( /* heap */)
        {
            var normalParam = NormalParam;
            if (normalParam == null || !normalParam.Setup)
            {
                var userSystem = User.GetSystem();
                /* If the UserHeap in system is not null and can fit us, use that heap instead. */
                SetupRomResourceParam();
            }

            var system = GetSystem();
            /* If system.EditorBuffer is not null, do a bunch of setup for that. */
        }

        private void SetupRomResourceParam( /* heap */)
        {
            if (NormalParam == null)
                return;

            var param = AllocResourceParam();
            NormalParam = param;

            var resourceBuffer = GetSystem().ResourceBuffer;
            var user = resourceBuffer.SearchResUserHeader(User.Name);
            if (!user.IsNull())
            {
                ref var resParam = ref resourceBuffer.RSP.Common;
                SetupResourceParam(
                    NormalParam,
                    user,
                    in resParam,
                    in resourceBuffer.PDT
                );
            }
            else
            {
                /* They do this, prob something ifdef'd out? */
                GetSystem();

                SetupResourceParam(
                    NormalParam,
                    ResourceBuffer.EmptyUserHeader,
                    in ResourceBuffer.EmptyRomResourceParam.Common,
                    in resourceBuffer.PDT
                );
            }
        }

        public bool HasGlobalPropertyTrigger()
        {
            var param = CurrentParam;
            if (param == null) 
                return false;

            if(!param.Setup)
                return false;

            var numProperty = param.User.Header.NumResProperty;
            if(numProperty == 0 ) 
                return false;

            for (var i = 0; i < numProperty; i++)
            {
                if (param.User.PropertyTableSpan[i].IsGlobalBool)
                    return true;
            }
            return false;
        }

        private static void SolveNeedObserveFlag(UserResourceParam param)
        {
            if(param.User.Header.NumCallTable == 0)
                return;

            var assetCalls = param.User.AssetCallTableSpan;
            for (var i = 0; i < assetCalls.Length; i++)
            {
                ref var assetCall = ref assetCalls[i];
                var parentIdx = assetCall.ParentIndex;
                if (parentIdx < 0 || !assetCalls[parentIdx].IsContainer)
                {
                    SolveNeedObserveFlagImpl(i, ref assetCall, param, param.User.Header);
                }
            }
        }

        private void SetupResourceParam(UserResourceParam param, Pointer<ResUserHeader> headerPtr,
            in CommonResourceParam commonParam, in ParamDefineTable pdt /* heap */)
        {
            ref var header = ref headerPtr.Ref;
            ResourceParamCreator.CreateUserBinParam(out param.User, headerPtr, in pdt);
            param.Common = commonParam;

            if (commonParam.NumLocalPropertyNameRefTable > 0)
            {
                param.PropertyNameIndexToPropertyIndex = new sbyte[commonParam.NumLocalPropertyNameRefTable];
                Array.Fill(param.PropertyNameIndexToPropertyIndex, (sbyte)-1);
            }

            if (header.NumCallTable > 0)
            {
                param.CallTables = new ResCallTable[header.NumCallTable];
                Array.Fill(param.CallTables, new ResCallTable());
            }

            if (header.NumResAction > 0)
            {
                param.ActionTriggers = new bool[header.NumResAction];
            }

            param.Accessor.HeaderPointer = headerPtr;

            if (User.PropertyDefinitionTable.Length > 0)
            {
                for (sbyte i = 0; i < User.PropertyDefinitionTable.Length; i++)
                {
                    var propertyDef = User.PropertyDefinitionTable[i];
                    if(propertyDef == null)
                        continue;

                    if (commonParam.NumLocalPropertyNameRefTable - 1 < 0)
                        continue;

                    var search1 = Utils.BinarySearch<Pointer<char>>(commonParam.LocalPropertyNameRefTableSpan, (v) => string.Compare(v.AsString(), propertyDef.Name, StringComparison.Ordinal));
                    if(search1 < 0)
                        continue;

                    param.PropertyNameIndexToPropertyIndex[search1] = i;

                    var search2 = Utils.BinarySearch<LocalPropertyRef>(param.User.LocalPropertyRefArrySpan, (v) => string.Compare(v.Name.AsString(), propertyDef.Name, StringComparison.Ordinal));
                    if(search2 < 0)
                        continue;

                    param.PropertyAssignedBitfield |= 1ul << i;
                }
            }

            if (param.User.ResUserHeader.Ref.NumCallTable > 0)
            {
                var actSpan = param.User.AssetCallTableSpan;
                for (var actIdx = 0; actIdx < actSpan.Length; actIdx++)
                {
                    ref var act = ref actSpan[actIdx];
                    var containerParam = ResourceUtil.GetResSwitchContainerParam(in act);
                    if(containerParam.IsNull())
                        continue;

                    var switchContainerParam = containerParam.GetForSwitch();
                    if(switchContainerParam.Ref.IsGlobal)
                        continue;

                    var localPropNameIdx = switchContainerParam.Ref.LocalPropertyNameIdx;
                    if(localPropNameIdx < 0)
                        continue;

                    var propIdx = param.PropertyNameIndexToPropertyIndex[localPropNameIdx];
                    if(propIdx < 0)
                        continue;

                    var propDef = User.PropertyDefinitionTable[propIdx];
                    if(propDef.Type != PropertyType.Enum)
                        continue;

                    var start = containerParam.Ref.ChildrenStartIndex;
                    var end = containerParam.Ref.ChildrenEndIndex;

                    if (start > end) 
                        continue;

                    for(var childIdx = start; childIdx < end; childIdx++)
                    {
                        ref var childAct = ref actSpan[childIdx];
                        if(childAct.Condition.IsNull())
                            continue;

                        var enumName = commonParam.LocalPropertyEnumNameRefTableSpan[childAct.Condition.GetForSwitch().Ref.LocalPropertyEnumNameIdx];
                        var enumValue = ((EnumPropertyDefinition)propDef).SearchEntryValueByKey(enumName.AsString());
                        param.CallTables[childIdx].EnumValue = (short)enumValue;
                    }
                }
            }

            if (header.NumResAction > 0)
            {
                var actionTableSpan = param.User.ActionTableSpan;
                var triggerTable = param.User.ActionTriggerTableSpan;
                for (var actionIdx = 0; actionIdx < actionTableSpan.Length; actionIdx++)
                {
                    ref var action = ref actionTableSpan[actionIdx];
                    var start = (int) action.TriggerStartIdx;

                    if (start > action.TriggerEndIdx) 
                        continue;

                    for (var triggerIdx = start; triggerIdx < action.TriggerEndIdx; triggerIdx++)
                    {
                        ref var trigger = ref triggerTable[triggerIdx];
                        if (trigger.TriggerType != ActionTriggerType.Zero)
                            continue;

                        param.ActionTriggers[actionIdx] = true;
                        break;
                    }
                }
            }

            OnSetupResourceParam_(param, in pdt);
            param.Setup = true;
        }

        private static bool SolveNeedObserveFlagImpl(int parentIndex, ref ResAssetCallTable parentCall, UserResourceParam param, ResUserHeader? header)
        {
            ref var assetCall = ref param.CallTables[parentIndex];

            if (parentCall.IsContainer)
            {
                var needToObserve = false;
                var containerPtr = ResourceUtil.GetResSwitchContainerParam(in parentCall);

                if (!containerPtr.IsNull() && containerPtr.Ref.ChildrenStartIndex > 0)
                {
                    ref var container = ref containerPtr.Ref;

                    var start = container.ChildrenStartIndex;
                    for (var childIdx = start; childIdx - 1 < container.ChildrenEndIndex; childIdx++)
                    {
                        ref var childAssetCall = ref param.User.AssetCallTableSpan[childIdx];
                        if (SolveNeedObserveFlagImpl(childIdx, ref childAssetCall, param, header))
                        {
                            needToObserve = true;
                        }
                    }
                }

                if (!needToObserve && parentCall.Duration > 0)
                {
                    assetCall.Flags &= ~2;
                    return false;
                }
            }
            else
            {
                if (parentCall.Duration > 0)
                {
                    if ((assetCall.Flags & 1) == 0)
                    {
                        assetCall.Flags &= ~2;
                        return false;
                    }
                }
            }
            assetCall.Flags |= 2;
            return true;
        }

        public abstract ResourceAccessor GetAccessor();
        public abstract ResourceAccessor GetAccessorPtr();
        public abstract System GetSystem();
        public abstract UserResourceParam AllocResourceParam(/* heap */);

        public virtual void FreeResourceParam(UserResourceParam param) { }

        public virtual void OnSetupResourceParam_(UserResourceParam param, in ParamDefineTable table /* heap */) {}
    }
}

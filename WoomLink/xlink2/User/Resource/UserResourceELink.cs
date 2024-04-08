using WoomLink.Ex;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.User.Instance;
using static WoomLink.xlink2.User.Resource.UserResourceParamELink;

namespace WoomLink.xlink2.User.Resource
{
    public class UserResourceELink : UserResource
    {
        public ResourceAccessorELink Accessor;
        public UserResourceELink(User user) : base(user)
        {
            Accessor = new ResourceAccessorELink(this, SystemELink.Instance);
        }

        public NullableRef<ResEsetGroupIdPair> GetSolvedAssetParameterELink(in ResAssetCallTable table)
        {
            var param = (UserResourceParamELink?) CurrentParam;

            if (param == null)
                return NullableRef<ResEsetGroupIdPair>.Null;
            if (!param.Setup)
                return NullableRef<ResEsetGroupIdPair>.Null;
            if (table.AssetId < 0)
                return NullableRef<ResEsetGroupIdPair>.Null;

            return new NullableRef<ResEsetGroupIdPair>(ref param.SolvedAssetParameterByAssetId[table.AssetId]);
        }

        public void SolveResourceForChangeEset()
        {
            var param = CurrentParam;
            if (param == null) 
                return;
            
            if(!param.Setup) 
                return;

            var instance = (UserInstanceELink) User.GetLeaderInstance()!;
            var elinkParam = (UserResourceParamELink) param;
            var pdt = GetSystem().GetParamDefineTable(ResMode);
            SolveAssetParam(elinkParam, pdt, instance.PtclAccessor);
            SolveNeedObserveFlag(CurrentParam!);
        }

        private void SolveAssetParam(UserResourceParamELink param, in ParamDefineTable pdt, PtclResourceAccessorELink? ptclAccessor)
        {
            if (param.Common.NameTablePointer == 0)
                return;

            ref var user = ref param.User.ResUserHeader.Ref;
            var system = GetSystem();
            if (user.NumCallTable <= 0)
                return;

            var ptclSystem = system.PtclSystem;
            var acts = param.User.AssetCallTableSpan;
            for (var i = 0; i < acts.Length; i++)
            {
                ref var act = ref acts[i];
                if (act.IsContainer)
                    continue;

                var assetBitFlagParam = Accessor.GetResParamFromAssetParamPos(act.ParamAsAsset, ResourceAccessorELink.AssetBitFlagIndex);
                if(!assetBitFlagParam.IsNull && (assetBitFlagParam.Ref.GetAsInt(in param.Common) & ResourceAccessorELink.IsNeedObserveMask) != 0)
                {
                    param.CallTables[i].IsNeedFade = true;
                }

                var assetNameParam = Accessor.GetResParamFromAssetParamPos(act.ParamAsAsset, ResourceAccessorELink.AssetNameIndex);
                Pointer<char> assetName;
                if (!assetNameParam.IsNull)
                {
                    assetName = assetNameParam.Ref.GetAsString(in param.Common);
                }
                else
                {
                    assetName = pdt.GetAssetParamDefaultValueString(ResourceAccessorELink.AssetNameIndex);
                }

                /* This seems redundant... */
                if (!assetName.IsNull)
                {
                    ptclAccessor ??= new PtclResourceAccessorELink();

                    if (ptclAccessor.SearchEmitterSetID(ptclSystem, assetName, out var resourceIndex, out var emitterId))
                    {
                        ref var pair = ref param.SolvedAssetParameterByAssetId[act.AssetId];
                        pair.EsetVal = new ResEset()
                        {
                            EmitterId = (short)emitterId,
                            ResourceIndex = (short)resourceIndex
                        };

                        /* Get the nn::vfx::Resource* from PtclSystem. Call IsNeedFade, if so set ACT's Flags accordingly. */

                        if (!param.CallTables[i].IsNeedFade)
                        {
                            var clipTypeParam = Accessor.GetResParamFromAssetParamPos(act.ParamAsAsset, ResourceAccessorELink.ClipTypeIndex);
                            ClipType clipType;
                            if (!clipTypeParam.IsNull)
                            {
                                clipType = (ClipType) clipTypeParam.Ref.GetAsInt(in param.Common);
                            }
                            else
                            {
                                clipType = (ClipType) pdt.GetAssetParamDefaultValueInt(ResourceAccessorELink.ClipTypeIndex);
                            }

                            if (clipType is ClipType.Unk1 or ClipType.Unk2)
                            {
                                GetSystem().AddError(Error.Type.OneTimeShoudBeClipNoneOrClipKill, User, "key[{0}],eset[{1}]", act.KeyName.AsString(), assetName);
                            }
                        }
                    }
                }

                var groupTable = system.GroupTable;
                var groupId = 0;
                if (groupTable != null)
                {
                    var groupNameParam = Accessor.GetResParamFromAssetParamPos(act.ParamAsAsset, ResourceAccessorELink.GroupNameIndex);
                    Pointer<char> groupName;
                    if (!groupNameParam.IsNull)
                    {
                        groupName = groupNameParam.Ref.GetAsString(in param.Common);
                    }
                    else
                    {
                        groupName = pdt.GetAssetParamDefaultValueString(ResourceAccessorELink.GroupNameIndex);
                    }

                    groupId = groupTable.GetId(groupName.AsString());
                    param.SolvedAssetParameterByAssetId[act.AssetId].GroupId = (byte)groupId;
                }

                if (!assetName.IsNull)
                {
                    /* They get this again, for some reason. */
                    assetBitFlagParam = Accessor.GetResParamFromAssetParamPos(act.ParamAsAsset, ResourceAccessorELink.AssetBitFlagIndex);
                    uint assetBitFlag;
                    if (!assetBitFlagParam.IsNull)
                    {
                        assetBitFlag = (uint) assetBitFlagParam.Ref.GetAsInt(in param.Common);
                    }
                    else
                    {
                        assetBitFlag = (uint)pdt.GetAssetParamDefaultValueInt(ResourceAccessorELink.AssetBitFlagIndex);
                    }

                    if ((assetBitFlag & ResourceAccessorELink.UnkMask) != 0)
                    {
                        /* TODO: OneEmitterMgr */
                    }
                }
            }
        }

        public override ResourceAccessor GetAccessor() => Accessor;

        public override ResourceAccessor GetAccessorPtr() => Accessor;
        public override SystemELink GetSystem() => SystemELink.Instance;

        public override UserResourceParam AllocResourceParam()
        {
            var param = new UserResourceParamELink
            {
                Accessor = Accessor
            };
            return param;
        }
    }
}

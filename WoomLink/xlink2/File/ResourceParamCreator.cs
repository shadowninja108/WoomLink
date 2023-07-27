using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WoomLink.sead;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.xlink2.File
{
    public class ResourceParamCreator
    {
        class BinAccessor
        {
            public Pointer<ResourceHeader> Start;
            /* Editor header */
            /* Resource header but lower half on 32-bit platforms. */
            public Pointer<ResAssetParam> ResAssetParamTable;
            public int NumAsset;
            public int NumTrigger;
        }   

        public static void CreateParamAndSolveResource(ref RomResourceParam romParam, Pointer<ResourceHeader> start, in ParamDefineTable pdt, System system)
        {
            var userStart = start.AtEnd<uint>();
            romParam.Data = start;
            ref var header = ref start.Ref;
            
            if (header.NumUser > 0)
            {
                romParam.UserDataHashes = userStart;
                romParam.UserDataPointers = userStart.Add(header.NumUser).AlignUp(Heap.PointerSize).Cast<Pointer<ResUserHeader>>();
            }

            romParam.NumUser = header.NumUser;

            var accessor = new BinAccessor
            {
                Start = start,
                ResAssetParamTable =
                    userStart.Add(start.Ref.NumUser)
                    .Cast<UintPointer>()
                    .Add(start.Ref.NumUser)
                    .AddBytes(pdt.TotalSize)
                    .Cast<ResAssetParam>()
                    .AlignUp(Heap.PointerSize),
                NumAsset = pdt.NumTotalAssetParams,
                NumTrigger = pdt.NumTriggerParams,
            };

            CreateCommonResourceParam(ref romParam.Common, accessor);
            /* TODO: dumpRomResource */
            SolveCommonResource(ref romParam.Common);

            for(var i = 0; i < romParam.NumUser; i++)
            {
                ref var pointer = ref romParam.UserDataPointers.Add(i).Ref;
                pointer.PointerValue += start.PointerValue;

                ref var user = ref pointer.Ref;
                if(!user.IsSetupBool)
                    SolveUserBin(pointer, ref romParam.Common, in pdt);
            }

            if (system.Field58)
                SolveAboutGlobalProperty(ref romParam, in pdt, system);
            
            romParam.Setup = true;
        }

        private static void CreateCommonResourceParam(ref CommonResourceParam param, BinAccessor accessor)
        {
            /* TODO: support editor here */
            ref var header = ref accessor.Start.Ref;
            param.NumResParam = header.NumResParam;
            param.NumResAssetParam = header.NumResAssetParam;
            param.NumResTriggerOverwriteParam = header.NumResTriggerOverwriteParam;
            param.NumLocalPropertyNameRefTable = header.NumLocalPropertyNameRefTable;
            param.NumLocalPropertyEnumNameRefTable = header.NumLocalPropertyEnumNameRefTable;
            param.NumDirectValueTable = header.NumDirectValueTable;
            param.NumRandomTable = header.NumRandomTable;
            param.NumCurveTable = header.NumCurveTable;
            param.NumCurvePointTable = header.NumCurvePointTable;
            param.ResAssetParamTable = accessor.ResAssetParamTable;

            param.TriggerOverwriteParamTable = accessor.Start.AddBytes(header.TriggerOverwriteParamTablePos).Cast<ResTriggerOverwriteParam>();

            var localPropertyNameRefTable = accessor.Start.AddBytes(header.LocalPropertyNameRefTablePos).Cast<Pointer<char>>();
            param.LocalPropertyNameRefTable = localPropertyNameRefTable;
            param.LocalPropertyEnumNameRefTable = localPropertyNameRefTable.Add(param.NumLocalPropertyNameRefTable);
            param.DirectValueTable = param.LocalPropertyEnumNameRefTable.Add(param.NumLocalPropertyEnumNameRefTable).Cast<uint>();
            param.RandomTable = param.DirectValueTable.Add(param.NumDirectValueTable).Cast<ResRandomCallTable>();
            param.CurveTable = param.RandomTable.Add(param.NumRandomTable).Cast<ResCurveCallTable>();
            param.CurvePointTable = param.CurveTable.Add(param.NumCurveTable).Cast<CurvePointTable>();

            param.ExRegionLower = accessor.Start.AddBytes(header.ExRegionPos).Cast<ResUserHeader>();
            param.ConditionTable = accessor.Start.AddBytes(header.ConditionTablePos).Cast<ResCondition>();
            param.NameTable = accessor.Start.AddBytes(header.NameTablePos).PointerValue;
        }
        
        private static void SolveCommonResource(ref CommonResourceParam param)
        {
            foreach (ref var x in param.LocalPropertyNameRefTableSpan)
            {
                x.PointerValue += param.NameTable;
            }

            foreach (ref var x in param.LocalPropertyEnumNameRefTableSpan)
            {
                x.PointerValue += param.NameTable;
            }

            foreach (ref var x in param.CurveTableSpan)
            {
                x.PropName.PointerValue += param.NameTable;
            }

            var conditionPtr = param.ConditionTable;
            while (conditionPtr.PointerValue < param.NameTable)
            {
                ref var condition = ref conditionPtr.Ref;

                if (condition.IsSwitch)
                {
                    var switchs = conditionPtr.GetForSwitch();

                    if(switchs.Ref.PropertyType == PropertyType.Enum)
                        switchs.GetStringValue().Ref.PointerValue += param.NameTable;
                }

                conditionPtr = conditionPtr.GetNext();
            }
        }

        private static void SolveUserBin(Pointer<ResUserHeader> start, ref CommonResourceParam commonParam, in ParamDefineTable pdt)
        {
            ref var header = ref start.Ref;
            
            /* TODO: why does this work??? */
            header.NumLocalProperty &= ushort.MaxValue;
            
            CreateUserBinParam(out var userParam, start, in pdt);

            foreach (ref var reff in userParam.LocalPropertyRefArrySpan)
            {
                reff.Name.PointerValue += commonParam.NameTable;
            }

            foreach (ref var act in userParam.AssetCallTableSpan) 
            {
                act.KeyName.PointerValue += commonParam.NameTable;
                act.KeyNameHash = HashCrc32.CalcStringHash(act.KeyName.AsString());

                if (act.IsContainer)
                {
                    if ((int)act.ParamPos == -1)
                    {
                        act.ParamPos = 0;
                    }
                    else
                    {
                        act.ParamPos += userParam.ContainerTablePos;

                        var param = act.ParamAsContainer;

                        if(param.Ref.Type != ContainerType.Switch
#if XLINK_VER_THUNDER
                            || param.Ref.Field1 != 0
#endif
                        )
                        {
                            if (param.Ref.Type == ContainerType.Mono)
                            {
                                var mono = param.GetForMono();
                                mono.Ref.Unk1.PointerValue += commonParam.NameTable;
                                mono.Ref.Unk2.PointerValue += commonParam.NameTable;
                            }
                        } 
                        else
                        {
                            var switchs = param.GetForSwitch();
                            switchs.Ref.WatchPropertyName.PointerValue += commonParam.NameTable;
                        }
                    }
                }
                else
                {
                    act.ParamPos += commonParam.ResAssetParamTable.PointerValue;
                }

                ref var condition = ref act.Condition;
                if ((int)condition.PointerValue == -1)
                    condition = Pointer<ResCondition>.Null;
                else
                    condition.PointerValue += commonParam.ConditionTable.PointerValue;
            }

            foreach (ref var resActionSlot in userParam.ResActionSlotTableSpan)
            {
                resActionSlot.Name.PointerValue += commonParam.NameTable;
            }

            foreach (ref var action in userParam.ActionTableSpan)
            {
                action.Name.PointerValue += commonParam.NameTable;
            }

            foreach (ref var actionTrigger in userParam.ActionTriggerTableSpan)
            {
                actionTrigger.AssetCtbPos.PointerValue += userParam.AssetCallTable.PointerValue;

                ref var overwriteParam = ref actionTrigger.OverwriteParam;
                if ((int)overwriteParam.PointerValue == -1)
                    overwriteParam = Pointer<ResTriggerOverwriteParam>.Null;
                else
                    overwriteParam.PointerValue += commonParam.TriggerOverwriteParamTable.PointerValue;

                if (actionTrigger.TriggerType == ActionTriggerType.Three)
                    actionTrigger.String.PointerValue += commonParam.NameTable;
            }

            foreach (ref var property in userParam.PropertyTableSpan)
            {
                property.WatchPropertyNamePos.PointerValue += commonParam.NameTable;
            }

            foreach (ref var resPropertyTrigger in userParam.ResPropertyTriggerTableSpan)
            {
                resPropertyTrigger.AssetCtb.PointerValue += userParam.AssetCallTable.PointerValue;

                if ((int)resPropertyTrigger.Condition.PointerValue == -1)
                    resPropertyTrigger.Condition = Pointer<ResCondition>.Null;
                else
                    resPropertyTrigger.Condition.PointerValue += commonParam.ConditionTable.PointerValue;

                ref var overwriteParam = ref resPropertyTrigger.OverwriteParam;
                if ((int)overwriteParam.PointerValue == -1)
                    overwriteParam.PointerValue = 0;
                else
                    overwriteParam.PointerValue += commonParam.TriggerOverwriteParamTable.PointerValue;
            }

            foreach (ref var resAlwaysTrigger in userParam.ResAlwaysTriggerTableSpan)
            {
                resAlwaysTrigger.AssetCtb.PointerValue += userParam.AssetCallTable.PointerValue;

                ref var overwriteParam = ref resAlwaysTrigger.OverwriteParamPos;
                if ((int)overwriteParam.PointerValue == -1)
                    overwriteParam.PointerValue = 0;
                else
                    overwriteParam.PointerValue += commonParam.TriggerOverwriteParamTable.PointerValue;
            }

            header.IsSetup = 1;
        }

        public static void CreateUserBinParam(out UserBinParam param, Pointer<ResUserHeader> start, in ParamDefineTable pdt)
        {
            ref var header = ref start.Ref;

            param = new();
            param.ResUserHeader = start;
            param.LocalPropertyRefArry = start.AtEnd<LocalPropertyRef>();
            param.UserParamArry = param.LocalPropertyRefArry.Add(header.NumLocalProperty).Cast<ResParam>();
            param.SortedAssetIdTable = param.UserParamArry.Add(pdt.NumAllParams).Cast<ushort>();

            var assetCtb = param.SortedAssetIdTable.Add(header.NumCallTable).Cast<ResAssetCallTable>();
            if ((header.NumCallTable & 1) != 0)
                assetCtb.PointerValue += (UintPointer)Unsafe.SizeOf<ushort>();

            param.AssetCallTable = assetCtb;

            if (header.NumCallTable != header.NumAsset)
            {
                param.ContainerTablePos = param.AssetCallTable.Add(header.NumCallTable).PointerValue;
            }

            param.ResActionSlotTable = start.AddBytes(header.TriggerTablePos).Cast<ResActionSlot>();
            param.ActionTable = param.ResActionSlotTable.Add(header.NumResActionSlot).Cast<ResAction>();
            param.ActionTriggerTable = param.ActionTable.Add(header.NumResAction).Cast<ResActionTrigger>();
            param.PropertyTable = param.ActionTriggerTable.Add(header.NumResActionTrigger).Cast<ResProperty>();
            param.ResPropertyTriggerTable = param.PropertyTable.Add(header.NumResProperty).Cast<ResPropertyTrigger>();
            param.ResAlwaysTriggerTable = param.ResPropertyTriggerTable.Add(header.NumResPropertyTrigger).Cast<ResAlwaysTrigger>();
        }

        public static void SolveAboutGlobalProperty(ref RomResourceParam param, in ParamDefineTable pdt, System system)
        {
            var curveCallTable = param.Common.CurveTable.AsSpan(param.Common.NumCurveTable);

            foreach (ref var curveCall in curveCallTable)
            {
                if(!curveCall.IsPropGlobalBool)
                    continue;

                if(curveCall.LocalPropertyNameIdx != -1)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(curveCall.PropName.AsString());

                if (idx == -1)
                {
                    //throw new Exception($"Property {curveCall.PropName.AsString()} not found in curve");
                    continue;
                }

                curveCall.LocalPropertyNameIdx = (short)idx;
            }

            var userBins = param.UserDataPointers.AsSpan(param.NumUser);
            foreach (var userBin in userBins) 
            {
                SolveUserBinAboutGlobalProperty(userBin, in pdt, system);
            }
        }

        private static void SolveUserBinAboutGlobalProperty(Pointer<ResUserHeader> bin, in ParamDefineTable pdt, System system)
        {
            CreateUserBinParam(out var param, bin, in pdt);

            var callTable = param.AssetCallTableSpan;
            foreach (ref var act in callTable)
            {
                if (!act.IsContainer)
                    continue;

                if((int)act.ParamPos == 0)
                    continue;

                var container = act.ParamAsContainer;
                if(container.Ref.Type != ContainerType.Switch
#if XLINK_VER_THUNDER
                   || container.Ref.Field1 == 0
#endif
                )
                    continue;

                ref var switchs = ref container.GetForSwitch().Ref;
                if(!switchs.IsGlobal)
                    continue;

                var idx = switchs.LocalPropertyNameIdx;
                if (idx == -1)
                {
                    idx = (short) system.SearchGlobalPropertyIndex(switchs.WatchPropertyName.AsString());

                    if (idx == -1)
                    {
                        //throw new Exception($"Property {switchs.WatchPropertyName.AsString()} is not in container {act.KeyName.AsString()}");
                        continue;
                    }
                }

                switchs.LocalPropertyNameIdx = idx;
                
                var def = system.GlobalPropertyDefinitions[idx];
                if(def.Type != PropertyType.Enum)
                    continue;

                var start = container.Ref.ChildrenStartIndex;
                var end = container.Ref.ChildrenEndIndex;

                if(start > end)
                    continue;

                for (var i = start; i <= end; i++)
                {
                    ref var childAct = ref callTable[i];
                    var childActCond = childAct.Condition;

                    if(childActCond.PointerValue == 0)
                        continue;

                    ref var childActCondSwitch = ref childActCond.GetForSwitch().Ref;
                    if(childActCondSwitch.IsSolvedBool)
                        continue;

                    childActCondSwitch.LocalPropertyEnumNameIdx = (short) ((EnumPropertyDefinition)def).SearchEntryValueByKey(childActCond.GetForSwitch().GetStringValue().AsString());
                    childActCondSwitch.IsSolved = 1;
                }
            }

            var propertyTable = param.PropertyTableSpan;
            var resPropertyTriggerTable = param.ResPropertyTriggerTableSpan;
            foreach (ref var x in propertyTable)
            {
                if(!x.IsGlobalBool)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(x.WatchPropertyNamePos.AsString());

                if(idx == -1)
                    continue;

                var def = system.GlobalPropertyDefinitions[idx];
                if(def.Type != PropertyType.Enum)
                    continue;

                var start = x.TriggerStartIdx;
                var end = x.TriggerEndIdx;
                if(start > end)
                    continue;

                for (var i = (int) start; i <= end; i++)
                {
                    ref var trigger = ref resPropertyTriggerTable[i];
                    var triggerCondition = trigger.Condition;

                    if(triggerCondition.PointerValue == 0)
                        continue;

                    ref var triggerConditionSwitch = ref triggerCondition.GetForSwitch().Ref;
                    if (triggerConditionSwitch.IsSolvedBool)
                        continue;

                    triggerConditionSwitch.LocalPropertyEnumNameIdx =
                        (short)system.SearchGlobalPropertyIndex(triggerCondition.GetForSwitch().GetStringValue().Ref.AsString());
                    triggerConditionSwitch.IsSolved = 1;
                }
            }
        }
    }
}

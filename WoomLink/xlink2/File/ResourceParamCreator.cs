using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.sead;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.xlink2.File
{
    public class ResourceParamCreator
    {
        private ref struct BinAccessor
        {
            public Pointer<ResourceHeader> RomHeader;
            public Pointer<EditorHeader> EditorHeader;
            public UintPointer Data;
            public Pointer<ResAssetParam> ResAssetParamTable;
            public int NumAsset;
            public int NumTrigger;

            public readonly int NumResParam                             => !RomHeader.IsNull ? RomHeader.Ref.NumResParam                        : EditorHeader.Ref.NumResParam;
            public readonly int NumResAssetParam                        => !RomHeader.IsNull ? RomHeader.Ref.NumResAssetParam                   : EditorHeader.Ref.NumResAssetParam;
            public readonly int NumResTriggerOverwriteParam             => !RomHeader.IsNull ? RomHeader.Ref.NumResTriggerOverwriteParam        : EditorHeader.Ref.NumResTriggerOverwriteParam;
            public readonly int NumLocalPropertyNameRefTable            => !RomHeader.IsNull ? RomHeader.Ref.NumLocalPropertyNameRefTable       : EditorHeader.Ref.NumLocalPropertyNameRefTable;
            public readonly int NumLocalPropertyEnumNameRefTable        => !RomHeader.IsNull ? RomHeader.Ref.NumLocalPropertyEnumNameRefTable   : EditorHeader.Ref.NumLocalPropertyEnumNameRefTable;
            public readonly int NumDirectValueTable                     => !RomHeader.IsNull ? RomHeader.Ref.NumDirectValueTable                : EditorHeader.Ref.NumDirectValueTable;
            public readonly int NumRandomTable                          => !RomHeader.IsNull ? RomHeader.Ref.NumRandomTable                     : EditorHeader.Ref.NumRandomTable;
            public readonly int NumCurveTable                           => !RomHeader.IsNull ? RomHeader.Ref.NumCurveTable                      : EditorHeader.Ref.NumCurveTable;
            public readonly int NumCurvePointTable                      => !RomHeader.IsNull ? RomHeader.Ref.NumCurvePointTable                 : EditorHeader.Ref.NumCurvePointTable;
            public readonly UintPointer TriggerOverwriteParamTablePos   => !RomHeader.IsNull ? RomHeader.Ref.TriggerOverwriteParamTablePos      : EditorHeader.Ref.TriggerOverwriteParamTablePos;
            public readonly UintPointer LocalPropertyNameRefTablePos    => !RomHeader.IsNull ? RomHeader.Ref.LocalPropertyNameRefTablePos       : EditorHeader.Ref.LocalPropertyNameRefTablePos;
            public readonly UintPointer ExRegionPos                     => !RomHeader.IsNull ? RomHeader.Ref.ExRegionPos                        : EditorHeader.Ref.ExDataRegionPos;
            public readonly UintPointer ConditionTablePos               => !RomHeader.IsNull ? RomHeader.Ref.ConditionTablePos                  : EditorHeader.Ref.ConditionTablePos;
            public readonly UintPointer NameTablePos                    => !RomHeader.IsNull ? RomHeader.Ref.NameTablePos                       : EditorHeader.Ref.NameTablePos;
        }   

        public static void CreateParamAndSolveResource(ref RomResourceParam romParam, UintPointer data, in ParamDefineTable pdt, System system)
        {
            var headerPointer = Pointer<ResourceHeader>.As(data);
            var userStart = headerPointer.AtEnd<uint>();
            romParam.Data = headerPointer;
            ref var header = ref headerPointer.Ref;
            
            
            if (header.NumUser > 0)
            {
                romParam.UserDataHashes = userStart;
                romParam.UserDataPointers = userStart.Add(header.NumUser).AlignUp(Ex.FakeHeap.PointerSize).Cast<Pointer<ResUserHeader>>();
            }

            romParam.NumUser = header.NumUser;

            var accessor = new BinAccessor
            {
                RomHeader = headerPointer,
                Data = data,
                ResAssetParamTable =
                    userStart.Add(header.NumUser)
                    .Cast<UintPointer>()
                    .Add(header.NumUser)
                    .AddBytes(pdt.TotalSize)
                    .Cast<ResAssetParam>()
                    .AlignUp(Ex.FakeHeap.PointerSize),
                NumAsset = pdt.NumTotalAssetParams,
                NumTrigger = pdt.NumTriggerParams,
            };

            CreateCommonResourceParam(ref romParam.Common, accessor);
            /* TODO: dumpRomResource */
            SolveCommonResource(ref romParam.Common);

            for(var i = 0; i < romParam.NumUser; i++)
            {
                ref var pointer = ref romParam.UserDataPointers.Add(i).Ref;
                pointer.PointerValue += data;

                ref var user = ref pointer.Ref;
                if(!user.IsSetupBool)
                    SolveUserBin(pointer, ref romParam.Common, in pdt);
            }

            if (system.Field58 != 0)
                SolveAboutGlobalProperty(ref romParam, in pdt, system);
            
            romParam.Setup = true;
        }

        public static void CreateParamAndSolveResource(ref EditorResourceParam editorParam, string name, UintPointer bin, uint binSize, in ParamDefineTable pdt, System system)
        {
            if (editorParam.Initialized)
            {
                if (editorParam.EditorBinSize != binSize)
                {
                    /* Free bin. */

                    editorParam.EditorBin = FakeHeap.Allocate((SizeT)binSize);
                    editorParam.EditorBinSize = binSize;
                } 
            }
            else
            {
                editorParam.Name = name;

                editorParam.EditorBin = FakeHeap.Allocate((SizeT)binSize);
                editorParam.EditorBinSize = binSize;
            }

            FakeHeap.Span(bin, (int)binSize).CopyTo(FakeHeap.Span(editorParam.EditorBin, (int)binSize));

            var header = Pointer<EditorHeader>.As(editorParam.EditorBin);
            editorParam.UserHeader = Pointer<ResUserHeader>.As(editorParam.EditorBin + header.Ref.UserBinPos);

            var accessor = new BinAccessor()
            {
                RomHeader = Pointer<ResourceHeader>.Null,
                EditorHeader = header,
                Data = editorParam.EditorBin,
                ResAssetParamTable = Pointer<ResAssetParam>.As(editorParam.EditorBin + (UintPointer)Unsafe.SizeOf<EditorHeader>()),
                NumAsset = pdt.NumTotalAssetParams,
                NumTrigger = pdt.NumTriggerParams,
            };
            CreateCommonResourceParam(ref editorParam.Common, in accessor);
            /* TODO: dumpEditorResource */
            SolveCommonResource(ref editorParam.Common);
            SolveUserBin(editorParam.UserHeader, ref editorParam.Common, in pdt);
            if(system.Field58 != 0)
                SolveAboutGlobalProperty(ref editorParam, in pdt, system);

            editorParam.Initialized = true;
        }

        private static void CreateCommonResourceParam(ref CommonResourceParam param, in BinAccessor accessor)
        {
            param.NumResParam = accessor.NumResParam;
            param.NumResAssetParam = accessor.NumResAssetParam;
            param.NumResTriggerOverwriteParam = accessor.NumResTriggerOverwriteParam;
            param.NumLocalPropertyNameRefTable = accessor.NumLocalPropertyNameRefTable;
            param.NumLocalPropertyEnumNameRefTable = accessor.NumLocalPropertyEnumNameRefTable;
            param.NumDirectValueTable = accessor.NumDirectValueTable;
            param.NumRandomTable = accessor.NumRandomTable;
            param.NumCurveTable = accessor.NumCurveTable;
            param.NumCurvePointTable = accessor.NumCurvePointTable;
            param.ResAssetParamTable = accessor.ResAssetParamTable;

            param.TriggerOverwriteParamTable = Pointer<ResTriggerOverwriteParam>.As(accessor.Data + accessor.TriggerOverwriteParamTablePos);

            var localPropertyNameRefTable = Pointer<Pointer<char>>.As(accessor.Data + accessor.LocalPropertyNameRefTablePos);
            param.LocalPropertyNameRefTable = localPropertyNameRefTable;
            param.LocalPropertyEnumNameRefTable = localPropertyNameRefTable.Add(param.NumLocalPropertyNameRefTable);
            param.DirectValueTable = param.LocalPropertyEnumNameRefTable.Add(param.NumLocalPropertyEnumNameRefTable).Cast<uint>();
            param.RandomTable = param.DirectValueTable.Add(param.NumDirectValueTable).Cast<ResRandomCallTable>();
            param.CurveTable = param.RandomTable.Add(param.NumRandomTable).Cast<ResCurveCallTable>();
            param.CurvePointTable = param.CurveTable.Add(param.NumCurveTable).Cast<CurvePointTable>();

            param.ExRegionPointer = Pointer<ResUserHeader>.As(accessor.Data + accessor.ExRegionPos);
            param.ConditionTable = Pointer<ResCondition>.As(accessor.Data + accessor.ConditionTablePos);
            param.NameTablePointer = accessor.Data + accessor.NameTablePos;
        }
        
        private static void SolveCommonResource(ref CommonResourceParam param)
        {
            foreach (ref var x in param.LocalPropertyNameRefTableSpan)
            {
                x.PointerValue += param.NameTablePointer;
            }

            foreach (ref var x in param.LocalPropertyEnumNameRefTableSpan)
            {
                x.PointerValue += param.NameTablePointer;
            }

            foreach (ref var x in param.CurveTableSpan)
            {
                x.PropName.PointerValue += param.NameTablePointer;
            }

            var conditionPtr = param.ConditionTable;
            while (conditionPtr.PointerValue < param.NameTablePointer)
            {
                ref var condition = ref conditionPtr.Ref;

                if (condition.IsSwitch)
                {
                    var switchs = conditionPtr.GetForSwitch();

                    if(switchs.Ref.PropertyType == PropertyType.Enum)
                        switchs.GetStringValue().Ref.PointerValue += param.NameTablePointer;
                }

                conditionPtr = conditionPtr.GetNext();
            }
        }

        private static void SolveUserBin(Pointer<ResUserHeader> start, ref CommonResourceParam commonParam, in ParamDefineTable pdt)
        {
            ref var header = ref start.Ref;
            
            CreateUserBinParam(out var userParam, start, in pdt);

            foreach (ref var reff in userParam.LocalPropertyRefArrySpan)
            {
                reff.Name.PointerValue += commonParam.NameTablePointer;
            }

            foreach (ref var act in userParam.AssetCallTableSpan) 
            {
                act.KeyName.PointerValue += commonParam.NameTablePointer;
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

                        var containerParamPtr = act.ParamAsContainer;
                        var containerParam = containerParamPtr.Ref;

                        if (containerParam.Type == ContainerType.Switch)
                        {
                            var switchs = containerParamPtr.GetForSwitch();
                            switchs.Ref.WatchPropertyName.PointerValue += commonParam.NameTablePointer;
                        }
#if  XLINK_VER_THUNDER || XLINK_VER_EXKING
                        else if (containerParam.Field1 != 0)
                        {
                            var special = containerParamPtr.GetSpecial();
                            special.Ref.Unk.PointerValue += commonParam.NameTablePointer;
                        } 
                        else if (containerParam.Type == ContainerType.Grid)
                        {
                            var mono = containerParamPtr.GetForGrid();
                            mono.Ref.Property1Name.PointerValue += commonParam.NameTablePointer;
                            mono.Ref.Property2Name.PointerValue += commonParam.NameTablePointer;
                        }
#endif
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
                resActionSlot.Name.PointerValue += commonParam.NameTablePointer;
            }

            foreach (ref var action in userParam.ActionTableSpan)
            {
                action.Name.PointerValue += commonParam.NameTablePointer;
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
                    actionTrigger.String.PointerValue += commonParam.NameTablePointer;
            }

            foreach (ref var property in userParam.PropertyTableSpan)
            {
                property.WatchPropertyNamePos.PointerValue += commonParam.NameTablePointer;
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
            param.SortedAssetIdTable = param.UserParamArry.Add(pdt.NumTotalUserParams).Cast<ushort>();

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
            var curveCallTable = param.Common.CurveTableSpan;

            foreach (ref var curveCall in curveCallTable)
            {
                if(!curveCall.IsPropGlobalBool)
                    continue;

                if(curveCall.LocalPropertyNameIdx != -1)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(curveCall.PropName.AsString());

                if (idx == -1)
                {
                    system.AddError(Error.Type.NotFoundProperty, null, "property[{0}(G)] in curve", curveCall.PropName.AsString());
                    continue;
                }

                curveCall.LocalPropertyNameIdx = (short)idx;
            }

            var userBins = param.UserDataPointers.AsSpan(param.NumUser);
            foreach (var userBin in userBins)
            {
                SolveUserBinAboutGlobalProperty(
                    userBin,
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                    ref param.Common,
#endif
                    in pdt,
                    system
                );
            }
        }

        public static void SolveAboutGlobalProperty(ref EditorResourceParam param, in ParamDefineTable pdt, System system)
        {
            var curveCallTable = param.Common.CurveTableSpan;

            foreach (ref var curveCall in curveCallTable)
            {
                if (!curveCall.IsPropGlobalBool)
                    continue;

                if (curveCall.LocalPropertyNameIdx != -1)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(curveCall.PropName.AsString());

                if (idx == -1)
                {
                    system.AddError(Error.Type.NotFoundProperty, null, "property[{0}(G)] in curve", curveCall.PropName.AsString());
                    continue;
                }

                curveCall.LocalPropertyNameIdx = (short)idx;
            }

            SolveUserBinAboutGlobalProperty(
                param.UserHeader,
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                ref param.Common,
#endif
                in pdt, 
                system
            );
        }

        private static void SolveUserBinAboutGlobalProperty(
            Pointer<ResUserHeader> bin,
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
            ref CommonResourceParam resParam,
#endif
            in ParamDefineTable pdt, 
            System system
            )
        {
            CreateUserBinParam(out var param, bin, in pdt);

            var callTable = param.AssetCallTableSpan;
            foreach (ref var act in callTable)
            {
                if (!act.IsContainer)
                    continue;

                if((int)act.ParamPos == 0)
                    continue;

                var containerPtr = act.ParamAsContainer;
                ref var container = ref containerPtr.Ref;

                if (
                    container.Type == ContainerType.Switch
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                    || container.Field1 != 0
#endif
                )
                {
                    ref var switchs = ref containerPtr.GetForSwitch().Ref;
                    if (!switchs.IsGlobal)
                        continue;

                    var idx = switchs.LocalPropertyNameIdx;
                    if (idx == -1)
                    {
                        idx = (short)system.SearchGlobalPropertyIndex(switchs.WatchPropertyName.AsString());

                        if (idx == -1)
                        {
                            system.AddError(Error.Type.NotFoundProperty, null, "property[{0}(G)] in swContainer({1})",
                                switchs.WatchPropertyName.AsString(), act.KeyName.AsString());
                            continue;
                        }
                    }

                    switchs.LocalPropertyNameIdx = idx;

                    var def = system.GlobalPropertyDefinitions[idx];
                    if (def.Type != PropertyType.Enum)
                        continue;

                    var start = containerPtr.Ref.ChildrenStartIndex;
                    var end = containerPtr.Ref.ChildrenEndIndex;

                    if (start > end)
                        continue;

                    for (var i = start; i <= end; i++)
                    {
                        ref var childAct = ref callTable[i];
                        var childActCond = childAct.Condition;

                        if (childActCond.PointerValue == 0)
                            continue;

                        ref var childActCondSwitch = ref childActCond.GetForSwitch().Ref;
                        if (childActCondSwitch.IsSolvedBool)
                            continue;

                        childActCondSwitch.LocalPropertyEnumNameIdx =
                            (short)((EnumPropertyDefinition)def).SearchEntryValueByKey(childActCond.GetForSwitch()
                                .GetStringValue().AsString());
                        childActCondSwitch.IsSolved = 1;
                    }
                }
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                else
                {
                    if (container.Type != ContainerType.Grid)
                        continue;

                    var gridPtr = containerPtr.GetForGrid();
                    ref var grid = ref gridPtr.Ref;

                    if (grid.IsProperty1Global && grid.Property1Index == -1)
                    {
                        var index = system.SearchGlobalPropertyIndex(grid.Property1Name.AsString());
                        if (index == -1)
                        {
                            system.AddError(Error.Type.NotFoundProperty, null, "property[{0}(G)] in gridContainer({1})",
                                grid.Property1Name.AsString(), act.KeyName.AsString());
                        }
                        else
                        {
                            grid.Property1Index = (short)index;
                            var propDef = system.GlobalPropertyDefinitions[index];
                            if (propDef.Type == PropertyType.Enum)
                            {
                                foreach (ref var value in gridPtr.GetProperty1ValueArray())
                                {
                                    var str = Pointer<char>.As(resParam.NameTablePointer + (UintPointer)value);
                                    value = ((EnumPropertyDefinition)propDef).SearchEntryValueByKey(str.AsString());
                                }
                            }
                        }
                    }

                    if (grid.IsProperty2Global && grid.Property2Index == -1)
                    {
                        var index = system.SearchGlobalPropertyIndex(grid.Property2Name.AsString());
                        if (index == -1)
                        {
                            system.AddError(Error.Type.NotFoundProperty, null, "property[{0}(G)] in gridContainer({1})",
                                grid.Property2Name.AsString(), act.KeyName.AsString());
                        }
                        else
                        {
                            grid.Property1Index = (short)index;
                            var propDef = system.GlobalPropertyDefinitions[index];
                            if (propDef.Type == PropertyType.Enum)
                            {
                                foreach (ref var value in gridPtr.GetProperty2ValueArray())
                                {
                                    var str = Pointer<char>.As(resParam.NameTablePointer + (UintPointer)value);
                                    value = ((EnumPropertyDefinition)propDef).SearchEntryValueByKey(str.AsString());
                                }
                            }
                        }
                    }
                }
#endif
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

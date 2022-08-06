using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Res.Ex;
using WoomLink.xlink2.File.Structs;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.xlink2.File
{
    public class ResourceParamCreator
    {
        class BinAccessor
        {
            public uint NumUser;
            public Stream Stream;
            public uint ResAssetParamTableOffset;
            public uint NumAsset;
            public uint NumTrigger;

            public ResourceHeader ResourceHeader;

            public string StringFromNameTable(uint pos)
            {
                using (Stream.TemporarySeek(ResourceHeader.NameTablePos + pos, SeekOrigin.Begin))
                    return new BinaryReader(Stream).ReadUtf8Z();
            }
        }   

        public static void CreateParamAndSolveResource(RomResourceParam romParam, Stream stream, ParamDefineTable pdt, System system)
        {
            ResourceHeader header = new();
            stream.Read(Utils.AsSpan(ref header));

            romParam.Header = header;
            romParam.UserDataHashes = stream.ReadArray<uint>(header.NumUser);
            romParam.UserBinOffsets = stream.ReadArray<uint>(header.NumUser).ToArray();

            romParam.NumUser = header.NumUser; 

            BinAccessor accessor = new()
            {
                NumUser = header.NumUser,
                Stream = stream,
                ResAssetParamTableOffset = (uint)(stream.Position + pdt.Header.Size),
                NumAsset = pdt.Header.NumAssetParams,
                NumTrigger = pdt.Header.NumTriggerParams
            };

            accessor.ResourceHeader = header;

            CreateCommonResourceParam(romParam, accessor);
            SolveCommonResource(romParam, accessor, pdt);

            romParam.UserBinParams = new UserBinParam[header.NumUser];

            for (var i = 0; i < header.NumUser; i++)
            {
                var offset = romParam.UserBinOffsets[i];
                using (stream.TemporarySeek(offset, SeekOrigin.Begin))
                {
                    UserBinParam param = new();
                    CreateUserBinParam(param, accessor, pdt);
                    SolveUserBin(param, romParam, pdt, accessor);

                    var hash = romParam.UserDataHashes[i];
                    var key = HashDb.FindByHash(hash);
                    param.Key = key;

                    romParam.UserBinParams[i] = param;
                }
            }
        }

        private static void CreateCommonResourceParam(CommonResourceParam param, BinAccessor accessor)
        {
            var stream = accessor.Stream;
            using (stream.TemporarySeek(accessor.ResAssetParamTableOffset, SeekOrigin.Begin))
            {
                param.AssetParamsPos = stream.Position;
                param.AssetParams = ResAssetParam.Read(stream, accessor.ResourceHeader.NumResAssetParam, accessor.NumAsset).ToArray();
            }

            param.TriggerOverwriteParamsPos = accessor.ResourceHeader.TriggerOverwriteParamTablePos;
            using (stream.TemporarySeek(accessor.ResourceHeader.TriggerOverwriteParamTablePos, SeekOrigin.Begin))
            {
                param.TriggerOverwriteParams = ResTriggerOverwriteParam
                    .Read(stream, accessor.ResourceHeader.NumResTriggerOverwriteParam, accessor.NumTrigger).ToArray();
            }

            using (stream.TemporarySeek(accessor.ResourceHeader.LocalPropertyNameRefTablePos, SeekOrigin.Begin))
            {
                param.LocalPropertyNameTable = 
                    stream.ReadArray<uint>(accessor.ResourceHeader.NumLocalPropertyNameRefTable)
                    .Select(accessor.StringFromNameTable).ToArray();
                param.LocalPropertyEnumNameTable =
                    stream.ReadArray<uint>(accessor.ResourceHeader.NumLocalPropertyEnumNameRefTable)
                    .Select(accessor.StringFromNameTable).ToArray();
                param.DirectValueTable =
                    stream.ReadArray<uint>(accessor.ResourceHeader.NumDirectValueTable);
                param.RandomTable =
                    stream.ReadArray<ResRandomCallTable>(accessor.ResourceHeader.NumRandomTable);
                param.CurveCallTable =
                    stream.ReadArray<ResCurveCallTable>(accessor.ResourceHeader.NumCurveTable)
                    .Select(x => new ResCurveCallTableEx(x)).ToArray();
                param.CurvePointTable =
                    stream.ReadArray<CurvePointTable>(accessor.ResourceHeader.NumCurvePointTable);
            }

            param.ConditionTablePos = accessor.ResourceHeader.ConditionTablePos;
            using (stream.TemporarySeek(accessor.ResourceHeader.ConditionTablePos, SeekOrigin.Begin))
            {
                param.ConditionTable = ResCondition.Read(stream, accessor.ResourceHeader.NameTablePos).ToArray();
            }

            param.NameTablePos = accessor.ResourceHeader.NameTablePos;
        }

        /* */
        private static void SolveCommonResource(CommonResourceParam param, BinAccessor accessor, ParamDefineTable pdt)
        {
            /* Normally LocalPropertyNameTable, LocalPropertyEnumNameTable, CurveCallTable and ConditionTable are relocated here. */

            /* These are not normally done. */
            foreach(var x in param.AssetParams)
                x.Solve(accessor.Stream, param, pdt);
            foreach(var x in param.TriggerOverwriteParams)
                x.Solve(accessor.Stream, param, pdt);

            foreach(var x in param.CurveCallTable)
                x.Solve(accessor.Stream, param);

            foreach(var x in param.ConditionTable)
                x.Solve(accessor.Stream, param);
        }

        /* Normally this takes ResUserHeader, CommonResourceParam, ParamDefineTable. */
        private static void SolveUserBin(UserBinParam userParam, CommonResourceParam commonParam, ParamDefineTable pdt, BinAccessor accessor)
        {
            /* Not normally done. */
            for (var i = 0; i < userParam.UserParamTable.Length; i++)
            {
                var x = userParam.UserParamTable[i];
                x.Solve(accessor.Stream, commonParam, pdt.UserParam[i]);
            }

            userParam.LocalPropertyStringTable = 
                 userParam.LocalPropertyRefTable
                 .Select(x => accessor.StringFromNameTable(x.NamePos))
                 .ToArray();

            userParam.AssetCallTableEx =
                userParam.AssetCallTable
                .Select(x => new ResAssetCallTableEx(x))
                .ToArray();

            userParam.ResActionSlotTableEx =
                userParam.ResActionSlotTable
                .Select(x => new ResActionSlotEx(x))
                .ToArray();

            userParam.ResActionTableEx =
                userParam.ResActionTable
                .Select(x => new ResActionEx(x))
                .ToArray();

            userParam.ResPropertyTableEx =
                userParam.ResPropertyTable
                .Select(x => new ResPropertyEx(x))
                .ToArray();

            userParam.ResPropertyTriggerTableEx =
                userParam.ResPropertyTriggerTable
                .Select(x => new ResPropertyTriggerEx(x))
                .ToArray();

            userParam.ResActionTriggerTableEx =
                userParam.ResActionTriggerTable
                .Select(x => new ResActionTriggerEx(x))
                .ToArray();


            foreach(var x in userParam.ResActionSlotTableEx)
                x.Solve(accessor.Stream, commonParam);

            foreach (var x in userParam.AssetCallTableEx)
                x.Solve(accessor.Stream, commonParam, userParam, pdt);

            foreach (var x in userParam.ResActionTableEx)
                x.Solve(accessor.Stream, commonParam);

            foreach (var x in userParam.ResPropertyTableEx)
                x.Solve(accessor.Stream, commonParam);

            foreach(var x in userParam.ResPropertyTriggerTableEx)
                x.Solve(accessor.Stream, commonParam, userParam, pdt);

            foreach (var x in userParam.ResActionTriggerTableEx)
                x.Solve(accessor.Stream, commonParam, userParam, pdt);

            if (userParam.ContainerParamTable != null)
            {
                foreach (var condition in userParam.ContainerParamTable)
                    condition.Solve(accessor.Stream, commonParam);
            }
        }

        /* A ResUserHeader would be passed instead of the accessor normally, but we aren't able to behave quite how they do. */
        private static void CreateUserBinParam(UserBinParam param, BinAccessor accessor, ParamDefineTable pdt)
        {
            var stream = accessor.Stream;

            using (stream.TemporarySeek())
            {
                stream.Read(Utils.AsSpan(ref param.Header));
                param.LocalPropertyRefTable = stream.ReadArray<LocalPropertyRef>(param.Header.NumLocalProperty);
                param.UserParamTable = ResParamEx.Read(stream, pdt.Header.NumUserParams).ToArray();
                param.SortedAssetIdTable = stream.ReadArray<ushort>(param.Header.NumCallTable);

                /* Align to 4 bytes. */
                if ((param.Header.NumCallTable & 1) != 0)
                    stream.Position += 2;

                param.AssetCallTablePos = stream.Position;
                param.AssetCallTable = stream.ReadArray<ResAssetCallTable>(param.Header.NumCallTable);

                if (param.Header.NumCallTable != param.Header.NumAsset)
                {
                    param.ContainerParamTablePos = stream.Position;

                    param.ContainerParamTable =
                        ResContainerParamEx.Read(stream, param.Header.NumCallTable - param.Header.NumAsset)
                            .ToArray();
                }
            }

            using (stream.TemporarySeek(param.Header.TriggerTablePos, SeekOrigin.Current))
            {
                param.ResActionSlotTable = stream.ReadArray<ResActionSlot>(param.Header.NumResActionSlot);
                param.ResActionTable = stream.ReadArray<ResAction>(param.Header.NumResAction);
                param.ResActionTriggerTable = stream.ReadArray<ResActionTrigger>(param.Header.NumResActionTrigger);
                param.ResPropertyTable = stream.ReadArray<ResProperty>(param.Header.NumResProperty);
                param.ResPropertyTriggerTable = stream.ReadArray<ResPropertyTrigger>(param.Header.NumResPropertyTrigger);
                param.ResAlwaysTriggerTable = stream.ReadArray<ResAlwaysTrigger>(param.Header.NumResAlwaysTrigger);
            }
        }

        public static void SolveAboutGlobalProperty(RomResourceParam param, ParamDefineTable pdt, System system)
        {
            foreach (var x in param.CurveCallTable)
            {
                if(!x.Internal.IsPropGlobalBool)
                    continue;

                if(x.Internal.LocalPropertyNameIdx == -1)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(x.PropName);

                if (idx == -1)
                {
                    throw new Exception($"Property {x.PropName} not found in curve");
                }

                x.Internal.LocalPropertyNameIdx = (short)idx;
            }

            foreach (var x in param.UserBinParams)
            {
                SolveUserBinAboutGlobalProperty(x, pdt, system);
            }
        }

        private static void SolveUserBinAboutGlobalProperty(UserBinParam param, ParamDefineTable pdt, System system)
        {
            foreach (var x in param.AssetCallTableEx)
            {
                if (!x.Internal.IsContainer)
                    continue;

                if(x.Internal.ParamStartPos == 0)
                    continue;
                
                var container = x.ContainerParam;
                if(container.Internal.Type != ContainerType.Switch || !container.SwitchValue.IsGlobal)
                    continue;

                var idx = container.SwitchValue.LocalPropertyNameIdx;
                if (idx == -1)
                {
                    idx = (short)system.SearchGlobalPropertyIndex(container.WatchPropertyName);

                    if (idx == -1)
                    {
                        // throw new Exception($"Property {container.WatchPropertyName} is not in container {x.KeyName}");
                        continue;
                    }
                }

                container.SwitchValue.LocalPropertyNameIdx = idx;
                
                var def = system.GlobalPropertyDefinitions[idx];
                if(def.Type != PropertyType.Enum)
                    continue;

                var start = container.Internal.ChildrenStartIndex;
                var end = container.Internal.ChildrenEndIndex;

                if(start > end)
                    continue;

                for (var i = start; i <= end; i++)
                {
                    var act = param.AssetCallTableEx[i];
                    var actCondition = act.Condition;

                    if(actCondition == null)
                        continue;

                    if(actCondition.NormalValue.IsSolvedBool)
                        continue;

                    actCondition.NormalValue.LocalPropertyEnumNameIdx = (short)((EnumPropertyDefinition)def).SearchEntryValueByKey(actCondition.ValueAsString);
                    actCondition.NormalValue.IsSolved = 1;
                }
            }

            foreach (var x in param.ResPropertyTableEx)
            {
                if(!x.Internal.IsGlobalBool)
                    continue;

                var idx = system.SearchGlobalPropertyIndex(x.WatchPropertyName);

                if(idx == -1)
                    continue;

                var def = system.GlobalPropertyDefinitions[idx];
                if(def.Type != PropertyType.Enum)
                    continue;

                var start = x.Internal.TriggerStartIdx;
                var end = x.Internal.TriggerEndIdx;
                if(start > end)
                    continue;

                for (var i = start; i <= end; i++)
                {
                    var trigger = param.ResPropertyTriggerTableEx[i];
                    var triggerCondition = trigger.Condition;

                    if(triggerCondition == null)
                        continue;

                    if(triggerCondition.NormalValue.IsSolvedBool)
                        continue;

                    triggerCondition.NormalValue.LocalPropertyEnumNameIdx =
                        (short)system.SearchGlobalPropertyIndex(triggerCondition.ValueAsString);
                    triggerCondition.NormalValue.IsSolved = 1;
                }
            }
        }
    }
}

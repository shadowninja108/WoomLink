using System.Diagnostics;
using System.Globalization;
using System.IO;
using WoomLink.sead;
using WoomLink.xlink2;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;
using WoomLink.xlink2.Properties.Enum;
using TextWriter = System.IO.TextWriter;

namespace WoomLink.Ex
{
    public class UserPrinter
    {
        public readonly TextWriter Writer = new StringWriter();

        private const bool ShowDefaultAssetParams = false;

        private int TabCount = 0;
        private bool AtNewLine = true;

        private void PrintTabs(int count)
        {
            for (var i = 0; i < count; i++)
            {
                Writer.Write("     ");
                //Console.Write("     ");
            }
        }

        private void Write(string value)
        {
            if(AtNewLine)
                PrintTabs(TabCount);
            Writer.Write(value);
            //Console.Write(value);
            AtNewLine = false;
        }

        private void WriteLine(string value)
        {
            if (AtNewLine)
                PrintTabs(TabCount);
            Writer.WriteLine(value);
            //Console.WriteLine(value);
            AtNewLine = true;
        }

        public void UnwrapOnce()
        {
            if (TabCount > 0)
            {
                TabCount--;
                WriteLine("}");
            }
        }

        public void Unwrap()
        {
            while (TabCount > 0)
            {
                UnwrapOnce();
            }
        }

        public void WriteCompare(CompareType compare)
        {
            switch (compare)
            {
                case CompareType.Equal:
                    Write("==");
                    break;
                case CompareType.GreaterThan:
                    Write(">");
                    break;
                case CompareType.GreaterThanOrEqual:
                    Write(">=");
                    break;
                case CompareType.LessThan:
                    Write("<");
                    break;
                case CompareType.LessThanOrEqual:
                    Write("<=");
                    break;
                case CompareType.NotEqual:
                    Write("!=");
                    break;
            }
        }

        public bool PrintCondition(xlink2.System system, in UserBinParam userParam, Pointer<ResAssetCallTable> actPtr)
        {
            ref var act = ref actPtr.Ref;
            var conditionPtr = act.Condition;
            ref var condition = ref conditionPtr.Ref;
            ref var parent = ref userParam.AssetCallTableSpan[act.ParentIndex];


            if (condition.IsSwitch && parent.ParamAsContainer.Ref.Type == ContainerType.Switch)
            {
                Write("if (");

                Write(parent.ParamAsContainer.GetForSwitch().Ref.WatchPropertyName.AsString());
                Write(" ");

                var forSwitch = conditionPtr.GetForSwitch();
                WriteCompare(forSwitch.Ref.CompareType);
                Write(" ");

                if (forSwitch.Ref.IsSolvedBool && forSwitch.Ref.IsGlobalBool)
                {
                    if (system.GlobalPropertyDefinitions[parent.ParamAsContainer.GetForSwitch().Ref.LocalPropertyNameIdx] is
                        EnumPropertyDefinition def)
                    {
                        Write(def.TypeName);
                        Write("::");
                    }
                }

                switch (forSwitch.Ref.PropertyType)
                {
                    case PropertyType.Enum:
                        Write(forSwitch.GetStringValue().Value.AsString());
                        break;
                    case PropertyType.S32:
                        Write(forSwitch.Ref.IntValue.ToString());
                        break;
                    case PropertyType.F32:
                        Write(forSwitch.Ref.FloatValue.ToString());
                        break;
                    default:
                        Write("?");
                        break;
                }

                WriteLine(") {");
                TabCount++;
                return true;
            }
            else if (condition.IsRandom)
            {
                WriteLine("random {");
                TabCount++;
                return true;
            }
            else if(parent.ParamAsContainer.Ref.IsRandom)
            {
                WriteLine($"random (weight: {conditionPtr.GetForRandom().Ref.Weight}) {{");
                TabCount++;
                return true;
            }

            return false;
        }

        public void PrintContainer(in UserBinParam param, Pointer<ResContainerParam> containerPtr)
        {
            ref var container = ref containerPtr.Ref;
            Debug.Assert(
                   container.Type == ContainerType.Mono 
                || container.Type == ContainerType.Switch
                || container.IsRandom 
                || container.Type == ContainerType.Sequence 
                || container.Type == ContainerType.Blend
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                || container.Type == ContainerType.Grid
#endif
#if XLINK_VER_EXKING
                || container.Type == ContainerType.Jump
#endif
            );
            
            switch (container.Type)
            {
                case ContainerType.Switch:
                    Write("switch (");
                    var switchs = containerPtr.GetForSwitch();
                    Write(switchs.Ref.WatchPropertyName.AsString());
                    WriteLine(") {");
                    break;
                    case ContainerType.Sequence:
                    WriteLine("seq {");
                    break;
                case ContainerType.Random:
                    WriteLine("random {");
                    break;
                case ContainerType.Random2:
                    WriteLine("random2 {");
                    break;
                case ContainerType.Blend:
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                    WriteLine($"blend({container.Field1}) {{");
#else
                    WriteLine("blend {");
#endif
                    break;
                case ContainerType.Mono:
                    WriteLine("mono {");
                    break;
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                case ContainerType.Grid:
                    var grid = containerPtr.GetForGrid();
                    WriteLine($"grid({grid.Ref.Property1Name.AsString()}, {grid.Ref.Property2Name.AsString()}) {{");
                    break;
#endif
#if XLINK_VER_EXKING
                case ContainerType.Jump:
                    WriteLine("jump {");
                    break;
#endif
                default:
                    WriteLine($"unk {(int)container.Type} {{}}");
                    return;
                }

            TabCount++;

            var start = container.ChildrenStartIndex;
            var end = container.ChildrenEndIndex;

            void PrintAssetCall(in UserBinParam param, Pointer<ResContainerParam> containerPtr, int idx)
            {
                ref var container = ref containerPtr.Ref;
                ref var call = ref param.AssetCallTableSpan[idx];

#if XLINK_VER_THUNDER || XLINK_VER_EXKING
                if (container.Type == ContainerType.Grid)
                {
                    var gridPtr = containerPtr.GetForGrid();
                    ref var grid = ref gridPtr.Ref;

                    /* TODO: */
                }
#endif

                Write(call.KeyName.AsString());

                if (container.IsRandom)
                {
                    Write($" |\t{call.Condition.GetForRandom().Ref.Weight}");
                }

                WriteLine(",");
            }

            if (end < start)
            {
                PrintAssetCall(in param, containerPtr, start);
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    PrintAssetCall(in param, containerPtr, i);
                }
            }
            UnwrapOnce();
        }

        public void PrintResParam(in CommonResourceParam commonParam, in ResParam param, in ParamDefine define)
        {
            switch (param.ReferenceType)
            {
                //case ValueReferenceType.Curve:
                //    ref var curve = ref commonParam.CurvePointTableSpan[(int)p.Value];
                //    Write($"X: {curve.X} | Y: {curve.Y}");
                //    break;
                //
                case ValueReferenceType.String:
                    Debug.Assert(define.Type == ParamType.String);
                    Write("\"");
                    Write(Pointer<char>.As(param.Value + commonParam.NameTablePointer).AsString());
                    Write("\"");
                    break;
                case ValueReferenceType.Curve:
                case ValueReferenceType.Direct:
                {
                    switch (define.Type)
                    {
                        case ParamType.Int:
                            Write(param.GetAsInt(in commonParam).ToString());
                            break;
                        case ParamType.Float:
                            Write(param.GetAsFloat(in commonParam).ToString(CultureInfo.CurrentCulture));
                            break;
                        case ParamType.Bool:
                            Write((param.GetAsInt(in commonParam) != 0).ToString());
                            break;
                        case ParamType.Enum:
                            Write("(Enum) ");
                            Write(param.GetAsInt(in commonParam).ToString());
                            break;
                        default:
                            Write(param.GetAsInt(in commonParam).ToString());
                            break;
                    }
                    break;
                }
                case ValueReferenceType.Bitfield:
                {
                    Write($"0b{param.Value:B8}");
                    break;
                }
            }

            if (
                param.ReferenceType != ValueReferenceType.Direct &&
                param.ReferenceType != ValueReferenceType.String
                //p.ReferenceType != ValueReferenceType.Curve
            )
            {
                Write($" ReferenceType:\t({param.ReferenceType})");
            }
        }

        public void PrintDefaultParam(in ParamDefine define)
        {
            switch (define.Type)
            {
                case ParamType.Int:
                    Write(define.DefaultValueAsInt.ToString());
                    break;
                case ParamType.Float:
                    Write(define.DefaultValueAsFloat.ToString());
                    break;
                case ParamType.Bool:
                    Write(define.DefaultValueAsInt != 0 ? "true" : "false");
                    break;
                case ParamType.Enum:
                    Write("TODO");
                    break;
                case ParamType.String:
                    Write("\"");
                    Write(define.DefaultValueAsString.AsString());
                    Write("\"");
                    break;
                case ParamType.Bitfield:
                    Write(define.DefaultValueAsInt.ToString());
                    break;
            }
        }

        public void PrintAssetParam(xlink2.System system, in CommonResourceParam commonParam, Pointer<ResAssetCallTable> actPtr)
        {
            WriteLine("run {");
            TabCount++;

            ref var act = ref actPtr.Ref;
            var pdt = system.ResourceBuffer.PDT;
            var assetPtr = act.ParamAsAsset;
            ref var asset = ref assetPtr.Ref;
            var assetParams = assetPtr.GetValuesSpan();

            for (var bitIdx = 0; bitIdx < system.ResourceBuffer.PDT.NumTotalAssetParams; bitIdx++)
            {
                if (asset.IsParamDefault((uint)bitIdx) && !ShowDefaultAssetParams)
                    continue;

                ref var def = ref pdt.AssetParamSpan[bitIdx];
                
                Write(def.Name.AsString());
                Write(" = ");

                if (!asset.IsParamDefault((uint)bitIdx))
                {
                    ref var p = ref assetParams[BitFlagUtil.CountRightOnBit64(asset.Bitfield, bitIdx) - 1];
                    PrintResParam(in commonParam, in p, in def);
                }
                else
                {
                    PrintDefaultParam(in def);
                }
                WriteLine(",");
            }
            UnwrapOnce();
        }

        public void Print(xlink2.System system, in CommonResourceParam commonParam, in UserBinParam userParam)
        {
            WriteLine("Params {");
            TabCount++;
            var uparams = userParam.UserParamArry.AsSpan(system.ResourceBuffer.PDT.NumTotalUserParams);
            for (var i = 0; i < uparams.Length; i++)
            {
                ref var p = ref uparams[i];
                ref var def = ref system.ResourceBuffer.PDT.UserParamSpan[i];

                Write(def.Name.AsString());
                Write(" = ");
                PrintResParam(in commonParam, in p, in def);
                WriteLine(",");
            }
            UnwrapOnce();

            WriteLine("Asset Calls {");
            TabCount++;
            for (var i = 0; i < userParam.Header.NumCallTable; i++)
            {
                var actPtr = userParam.AssetCallTable.Add(i);
                ref var act = ref actPtr.Ref;
                
                Write(act.KeyName.AsString());
                WriteLine(" {");
                TabCount++;
                var isCondition = !act.Condition.IsNull;

                if (isCondition)
                {
                    isCondition = PrintCondition(system, in userParam, actPtr);
                }

                if (act.IsContainer)
                {
                    PrintContainer(in userParam, act.ParamAsContainer);
                } 
                else
                {
                    PrintAssetParam(system, in commonParam, actPtr);
                }

                if(isCondition)
                    UnwrapOnce();
                UnwrapOnce();
            }
            UnwrapOnce();
        }
    }
}

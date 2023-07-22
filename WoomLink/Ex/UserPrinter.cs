using System;
using System.Diagnostics;
using System.IO;
using WoomLink.sead;
using WoomLink.xlink2;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.Ex
{
    public class UserPrinter
    {
        public static readonly TextWriter _writer = new StringWriter();

        private const bool ShowDefaultAssetParams = false;

        private int TabCount = 0;
        private bool AtNewLine = true;

        private static void PrintTabs(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _writer.Write("     ");
                Console.Write("     ");
            }
        }

        private void Write(string value)
        {
            if(AtNewLine)
                PrintTabs(TabCount);
            _writer.Write(value);
            Console.Write(value);
            AtNewLine = false;
        }

        private void WriteLine(string value)
        {
            if (AtNewLine)
                PrintTabs(TabCount);
            _writer.WriteLine(value);
            Console.WriteLine(value);
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

        public void PrintCondition(xlink2.System system, ref CommonResourceParam commonParam, ref UserBinParam userParam, Pointer<ResAssetCallTable> actPtr)
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
                        Write(forSwitch.GetIntValue().ToString());
                        break;
                    case PropertyType.F32:
                        Write(forSwitch.GetFloatValue().ToString());
                        break;
                    default:
                        Write("?");
                        break;
                }

                WriteLine(") {");
                TabCount++;
            }
            else if(parent.ParamAsContainer.Ref.IsRandom)
            {
                WriteLine($"random (weight: {conditionPtr.GetForRandom().Ref.Weight}) {{");
                TabCount++;
            }
        }

        public void PrintContainer(ref UserBinParam param, Pointer<ResContainerParam> containerPtr)
        {
            ref var container = ref containerPtr.Ref;
            Debug.Assert(
                container.Type == ContainerType.Mono ||
                container.Type == ContainerType.Switch ||
                container.IsRandom ||
                container.Type == ContainerType.Sequence ||
                container.Type == ContainerType.Blend ||
                container.Type == ContainerType.Unk
            );
            if (container.Type == ContainerType.Switch)
            {
                Write("switch (");
                var switchs = containerPtr.GetForSwitch();
                Write(switchs.Ref.WatchPropertyName.AsString());
                WriteLine(") {");
            } 
            else switch (container.Type)
            {
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
                    WriteLine("blend {");
                    break;
                case ContainerType.Mono:
                    WriteLine("mono {");
                    break;
                case ContainerType.Unk:
                    WriteLine("unk {}");
                    UnwrapOnce();
                    return;
            }

            TabCount++;

            var start = container.ChildrenStartIndex;
            var end = container.ChildrenEndIndex;

            void PrintAssetCall(ref UserBinParam param, ref ResContainerParam container, int idx)
            {
                ref var call = ref param.AssetCallTableSpan[idx];
                Write(call.KeyName.AsString());

                if (container.IsRandom)
                {
                    Write($" |\t{call.Condition.GetForRandom().Ref.Weight}");
                }

                WriteLine(",");
            }

            if (end < start)
            {
                PrintAssetCall(ref param, ref container, start);
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    PrintAssetCall(ref param, ref container, i);
                }

                UnwrapOnce();
            }
            UnwrapOnce();
        }

        public void PrintAssetParam(xlink2.System system, ref CommonResourceParam commonParam, ref UserBinParam userParam, Pointer<ResAssetCallTable> actPtr)
        {
            WriteLine("run {");
            TabCount++;

            ref var act = ref actPtr.Ref;
            var pdt = system.ResourceBuffer.PDT;
            var assetPtr = act.ParamAsAsset;
            ref var asset = ref assetPtr.Ref;
            var assetParams = assetPtr.GetValuesSpan();
            var condition = act.Condition;

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

                    switch (p.ReferenceType)
                    {
                        //case ValueReferenceType.Curve:
                        //    ref var curve = ref commonParam.CurvePointTableSpan[(int)p.Value];
                        //    Write($"X: {curve.X} | Y: {curve.Y}");
                        //    break;
                        //
                        case ValueReferenceType.String:
                            Debug.Assert(def.Type == ParamType.String);
                            Write("\"");
                            Write(Pointer<char>.As(p.Value + commonParam.NameTable).AsString());
                            Write("\"");
                            break;
                        case ValueReferenceType.Curve:
                        case ValueReferenceType.Direct:
                        {
                            switch (def.Type)
                            {
                                case ParamType.Int:
                                    Write(commonParam.DirectValueTableSpanAsInts[(int)p.Value].ToString());
                                    break;
                                case ParamType.Float:
                                    Write(commonParam.DirectValueTableSpanAsFloats[(int)p.Value].ToString());
                                    break;
                                case ParamType.Bool:
                                    Write((commonParam.DirectValueTableSpan[(int)p.Value] != 0).ToString());
                                    break;
                                case ParamType.Enum:
                                    Write(commonParam.DirectValueTableSpanAsInts[(int)p.Value].ToString());
                                    Write(" (TODO)");
                                    break;
                                default:
                                    Write(commonParam.DirectValueTableSpan[(int)p.Value].ToString());
                                    break;
                                }
                            break;
                        }
                    }

                    if (
                        p.ReferenceType != ValueReferenceType.Direct && 
                        p.ReferenceType != ValueReferenceType.String
                        //p.ReferenceType != ValueReferenceType.Curve
                    )
                    {
                        Write($" ReferenceType:\t({p.ReferenceType})");
                    }
                }
                else
                {
                    switch (def.Type)
                    {
                        case ParamType.Int:
                            Write(def.DefaultValueAsInt.ToString());
                            break;
                        case ParamType.Float:
                            Write(def.DefaultValueAsFloat.ToString());
                            break;
                        case ParamType.Bool:
                            Write(def.DefaultValueAsInt != 0 ? "true" : "false");
                            break;
                        case ParamType.Enum:
                            Write("TODO");
                            break;
                        case ParamType.String:
                            Write("\"");
                            Write(def.DefaultValueAsString.AsString());
                            Write("\"");
                            break;
                        case ParamType.Byte:
                            Write(def.DefaultValueAsInt.ToString());
                            break;
                    }
                }
                WriteLine(",");
            }
            UnwrapOnce();
        }

        public void Print(xlink2.System system, ref CommonResourceParam commonParam, ref UserBinParam userParam)
        {
            for (var i = 0; i < userParam.Header.NumCallTable; i++)
            {
                var actPtr = userParam.AssetCallTable.Add(i);
                ref var act = ref actPtr.Ref;
                
                Write(act.KeyName.AsString());
                WriteLine(" {");
                TabCount++;

                if (!act.Condition.IsNull())
                {
                    PrintCondition(system, ref commonParam, ref userParam, actPtr);
                }

                if (act.IsContainer)
                {
                    PrintContainer(ref userParam, act.ParamAsContainer);
                } else
                {
                    PrintAssetParam(system, ref commonParam, ref userParam, actPtr);
                }

                Unwrap();
                WriteLine("");
                TabCount = 0;
            }
        }
    }
}

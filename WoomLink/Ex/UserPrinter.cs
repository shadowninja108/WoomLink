using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Res.Ex;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink.Ex
{
    public class UserPrinter
    {
        private const bool ShowDefaultAssetParams = false;

        private int TabCount = 0;
        private bool AtNewLine = true;

        private static void PrintTabs(int count)
        {
            for (var i = 0; i < count; i++)
                Console.Write("     ");
        }

        private void Write(string value)
        {
            if(AtNewLine)
                PrintTabs(TabCount);
            Console.Write(value);
            AtNewLine = false;
        }

        private void WriteLine(string value)
        {
            if (AtNewLine)
                PrintTabs(TabCount);
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

        public void PrintCondition(xlink2.System system, UserBinParam param, ResAssetCallTableEx act)
        {
            var condition = act.Condition;
            var parent = param.AssetCallTableEx[act.Internal.ParentIndex];

            Debug.Assert(condition.IsNormal);
            Debug.Assert(parent.ContainerParam.IsSwitch);

            Write("if (");

            Write(parent.ContainerParam.WatchPropertyName);
            Write(" ");

            WriteCompare(condition.NormalValue.CompareType);
            Write(" ");

            if (condition.NormalValue.IsSolvedBool && condition.NormalValue.IsGlobal)
            {
                if (system.GlobalPropertyDefinitions[parent.ContainerParam.SwitchValue.LocalPropertyNameIdx] is EnumPropertyDefinition def)
                {
                    Write(def.TypeName);
                    Write("::");
                }
            }

            switch (condition.NormalValue.PropertyType)
            {
                case PropertyType.Enum:
                    Write(condition.ValueAsString);
                    break;
                case PropertyType.S32:
                    Write(condition.ValueAsInt.ToString());
                    break;
                case PropertyType.F32:
                    Write(condition.ValueAsFloat.ToString());
                    break;
                default:
                    Write("?");
                    break;
            }
            
            WriteLine(") {");
            TabCount++;
        }

        public void PrintContainer(UserBinParam param, ResContainerParamEx container)
        {
            Debug.Assert(
                container.IsSwitch ||
                container.Internal.Type == ContainerType.Sequence ||
                container.Internal.Type == ContainerType.Random ||
                container.Internal.Type == ContainerType.Blend
            );

            if (container.IsSwitch)
            {
                Write("switch (");
                Write(container.WatchPropertyName);
                WriteLine(") {");
            } 
            else if (container.Internal.Type == ContainerType.Sequence)
            {
                WriteLine("seq ()");
            } 
            else if (container.Internal.Type == ContainerType.Random)
            {
                Write("random (min: ");
            } else if (container.Internal.Type == ContainerType.Blend)
            {
                WriteLine("blend {");
            }

            TabCount++;

            var start = container.Internal.ChildrenStartIndex;
            var end = container.Internal.ChildrenEndIndex;

            if (end < start)
            {
                Write(param.AssetCallTableEx[start].KeyName);
                WriteLine(",");
            }
            else
            {
                for (var i = start; i <= end; i++)
                {
                    var call = param.AssetCallTableEx[i];
                    Write(call.KeyName);
                    WriteLine(",");
                }

                UnwrapOnce();
            }
            UnwrapOnce();
        }

        public void PrintAssetParam(xlink2.System system, UserBinParam param, ResAssetCallTableEx act)
        {
            WriteLine("run {");
            TabCount++;

            var pdt = system.ResourceBuffer.PDT;
            var asset = act.AssetParam;
            var condition = act.Condition;

            for (var i = 0; i < asset.Params.Length; i++)
            {
                var p = asset.Params[i];
                var def = pdt.AssetParam[i];


                if (p == null && !ShowDefaultAssetParams)
                    continue;

                Write(def.Name);
                Write(" = ");

                if (p.ReferenceType != ValueReferenceType.Direct && p.ReferenceType != ValueReferenceType.String)
                {
                    Write("(TODO)");
                } 
                else if (p != null)
                {
                    switch (def.Type)
                    {
                        case ParamType.Int:
                            Write(p.DirectValueAsUInt.ToString());
                            break;
                        case ParamType.Float:
                            Write(p.DirectValueAsFloat.ToString());
                            break;
                        case ParamType.Bool:
                            Write(p.DirectValueAsBool.ToString());
                            break;
                        case ParamType.Enum:
                            Write(p.DirectValueAsUInt.ToString());
                            Write(" (TODO)");
                            break;
                        case ParamType.String:
                            Write("\"");
                            Write(p.DirectValueAsString);
                            Write("\"");
                            break;
                        case ParamType.Byte:
                            break;
                    }
                }
                else
                {
                    switch (def.Type)
                    {
                        case ParamType.Int:
                            Write(def.DefaultValue.ToString());
                            break;
                        case ParamType.Float:
                            Write(def.DefaultValueAsFloat.ToString());
                            break;
                        case ParamType.Bool:
                            Write(def.DefaultValue != 0 ? "true" : "false");
                            break;
                        case ParamType.Enum:
                            Write("TODO");
                            break;
                        case ParamType.String:
                            Write("\"");
                            Write(def.DefaultValueAsString);
                            Write("\"");
                            break;
                        case ParamType.Byte:
                            Write(def.DefaultValue.ToString());
                            break;
                    }
                }
                WriteLine(",");
            }
            UnwrapOnce();
        }

        public void Print(xlink2.System system, UserBinParam param)
        {
            foreach (var x in param.AssetCallTableEx)
            {
                Write(x.KeyName);
                WriteLine(" {");
                TabCount++;

                if (x.Condition != null)
                {
                    PrintCondition(system, param, x);
                }

                if (x.ContainerParam != null)
                {
                    PrintContainer(param, x.ContainerParam);
                }

                if (x.AssetParam != null)
                {
                    PrintAssetParam(system, param, x);
                }

                Unwrap();
                WriteLine("");
                TabCount = 0;
            }
        }
    }
}

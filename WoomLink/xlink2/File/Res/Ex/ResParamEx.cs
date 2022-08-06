using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResParamEx
    {
        public ValueReferenceType ReferenceType;
        public uint Value;

        public string DirectValueAsString;
        public bool? DirectValueAsBool;
        public uint? DirectValueAsUInt;
        public float? DirectValueAsFloat;

        public ResParamEx(uint value)
        {
            /* Upper 8 bits. */
            ReferenceType = (ValueReferenceType)(value >> 24 & byte.MaxValue);
            /* Lower 24 bits */
            Value = value & (1 << 24) - 1;
        }

        public static IEnumerable<ResParamEx> Read(Stream stream, uint num)
        {
            var reader = new BinaryReader(stream);
            for (var i = 0; i < num; i++)
                yield return new ResParamEx(reader.ReadUInt32());
        }

        /* Normally done at access time. */
        public void Solve(Stream stream, CommonResourceParam param, ParamDefineTable.ParamDefineEx define)
        {
            switch (ReferenceType)
            {
                case ValueReferenceType.Direct:
                {
                    switch (define.Type)
                    {
                        case ParamType.Int:
                        {
                            DirectValueAsUInt = param.DirectValueTable[Value];
                            break;
                        }

                        case ParamType.Float:
                        {
                            DirectValueAsFloat = BitConverter.ToSingle(Utils.AsSpan(ref param.DirectValueTable[Value]));
                            break;
                        }

                        case ParamType.Bool:
                        {
                            DirectValueAsBool = param.DirectValueTable[Value] != 0;
                            break;
                        }
                        default:
                            break;
                    }
                    break;
                }
                case ValueReferenceType.String:
                {
                    Debug.Assert(define.Type == ParamType.String);
                    using (stream.TemporarySeek(param.NameTablePos + Value, SeekOrigin.Begin))
                        DirectValueAsString = new BinaryReader(stream).ReadUtf8Z();
                    break;
                }
            }
        }
    }
}

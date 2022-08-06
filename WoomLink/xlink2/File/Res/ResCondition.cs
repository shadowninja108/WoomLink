using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    public class ResCondition
    {
        public ContainerType Type;

        [StructLayout(LayoutKind.Sequential, Size = 0x4)]
        public struct ForRandom
        {
            public float Weight;
        }

        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        public struct ForNormal
        {
            public PropertyType PropertyType;
            public CompareType CompareType;
            public int Value;
            public short LocalPropertyEnumNameIdx;
            public byte IsSolved;
            public bool IsGlobal;

            public bool IsSolvedBool => IsSolved != 0;
        }

        public ForRandom RandomValue;
        public ForNormal NormalValue;

        public string ValueAsString;
        public int ValueAsInt => NormalValue.Value;
        public float ValueAsFloat => BitConverter.Int32BitsToSingle(ValueAsInt);

        public bool IsRandom => Type == ContainerType.Random || Type == ContainerType.Random2;
        public bool IsNormal => !IsRandom;

        public ResCondition(Stream stream)
        {
            var reader = new BinaryReader(stream);
            Type = (ContainerType)reader.ReadUInt32();

            if (IsRandom)
            {
                stream.Read(Utils.AsSpan(ref RandomValue));
            }
            else if (IsNormal)
            {
                stream.Read(Utils.AsSpan(ref NormalValue));
            }
        }

        public void Solve(Stream stream, CommonResourceParam param)
        {
            if (!IsNormal)
                return;

            if (NormalValue.PropertyType != PropertyType.Enum)
                return;

            using (stream.TemporarySeek(param.NameTablePos + NormalValue.Value, SeekOrigin.Begin))
                ValueAsString = new BinaryReader(stream).ReadUtf8Z();
        }

        public static IEnumerable<ResCondition> Read(Stream stream, uint end)
        {
            while (stream.Position < end)
            {
                yield return new ResCondition(stream);
            }
        }
    }
}

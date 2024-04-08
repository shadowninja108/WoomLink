using System.Runtime.InteropServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res
{
    [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
    public struct ResParam
    {
        [FieldOffset(0x0)]
        public UInt24 Value24;
        [FieldOffset(0x3)]
        public ValueReferenceType ReferenceType;

        public readonly uint Value => Value24.Value;

        public readonly int GetAsInt(in CommonResourceParam param) => param.DirectValueTableSpanAsInts[(int)Value];
        public readonly float GetAsFloat(in CommonResourceParam param) => param.DirectValueTableSpanAsFloats[(int)Value];

        public readonly Pointer<char> GetAsString(in CommonResourceParam param) => Pointer<char>.As(param.NameTablePointer + Value);

        public readonly ref ResCurveCallTable GetAsCurve(in CommonResourceParam param) => ref param.CurveTableSpan[(int)Value];
        public readonly ref ResRandomCallTable GetAsRandom(in CommonResourceParam param) => ref param.RandomTableSpan[(int)Value];
    }
}

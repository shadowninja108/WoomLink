using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Structs
{

    [StructLayout(LayoutKind.Sequential)]
    public struct ParamDefine
    {
        public Pointer<char> Name;
        public ParamType Type;
        public Pointer<char> DefaultValueAsString;

        public readonly float DefaultValueAsFloat
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(Type == ParamType.Float);
                return BitConverter.UInt32BitsToSingle((uint)DefaultValueAsString.PointerValue);
            }
        }

        public readonly int DefaultValueAsInt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(Type != ParamType.Float && Type != ParamType.Enum && Type != ParamType.String);
                return (int)DefaultValueAsString.PointerValue;
            }
        }
    }

}

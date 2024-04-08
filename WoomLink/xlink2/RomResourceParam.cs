using System;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2
{
    public struct RomResourceParam
    {
        public CommonResourceParam Common;
        public Pointer<ResourceHeader> Data;
        public Pointer<uint> UserDataHashes;
        public Pointer<Pointer<ResUserHeader>> UserDataPointers;
        public int NumUser;
        public bool Setup;

        public Span<uint> UserDataHashesSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (NumUser <= 0) return [];

                return UserDataHashes.AsSpan(NumUser);

            }
        }
        public Span<Pointer<ResUserHeader>> UserDataPointersSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (NumUser <= 0) return [];

                return UserDataPointers.AsSpan(NumUser);

            }
        }
    }
}

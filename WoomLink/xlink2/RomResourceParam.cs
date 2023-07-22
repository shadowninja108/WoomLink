using System;
using System.Runtime.CompilerServices;
using WoomLink.xlink2.File;
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
            get => UserDataHashes.AsSpan(NumUser);
        }
        public Span<Pointer<ResUserHeader>> UserDataPointersSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => UserDataPointers.AsSpan(NumUser);
        }
    }
}

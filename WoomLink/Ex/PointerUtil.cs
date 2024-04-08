using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WoomLink.sead;

namespace WoomLink.Ex
{
    public static class PointerUtil
    {
        public static ref byte GetArenaStart()
        {
            return ref HeapMgr.Arena!.Data![0];
        }

        public static Span<T> AsSpan<T>(in T self) where T : struct
        {
            return new Span<T>(ref Unsafe.AsRef(in self));
        }

        public static ref byte ToByteRef<T>(Span<T> self) where T : struct
        {
            return ref MemoryMarshal.AsBytes(self)[0];
        }

        public static UintPointer AsRawPtr<T>(in T self) where T : struct
        {
            return (UintPointer)Unsafe.ByteOffset(
                ref GetArenaStart(), 
                ref ToByteRef(AsSpan(in self))
            );
        }

        public static SizeT Subtract<T1, T2>(in T1 obj1, in T2 obj2) where T1 : struct where T2 : struct
        {
            return (SizeT) Unsafe.ByteOffset(ref ToByteRef(AsSpan(obj2)), ref ToByteRef(AsSpan(obj1)));
        }

        public static Pointer<T> AsPtr<T>(in T self) where T : struct
        {
            return Pointer<T>.As(AsRawPtr(in self));
        }

        public static NullableRef<T> AsNullableRef<T>(ref T self) where T : struct
        {
            return new NullableRef<T>(ref self);
        }

        public static NullableRef<T> AsNullableRef<T>(Pointer<T> pointer) where T : struct
        {
            return AsNullableRef(ref pointer.Ref);
        }
    }
}

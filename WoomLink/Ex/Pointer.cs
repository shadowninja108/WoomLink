#if !XLINK_VER_BLITZ && !XLINK_VER_THUNDER && !XLINK_VER_PARK && !XLINK_VER_EXKING
#error Invalid XLink version target.
#endif

#if XLINK_ARCH_32
global using UintPointer = System.UInt32;
global using IntPointer = System.Int32;
global using SizeT = System.Int32;
#elif XLINK_ARCH_64
global using UintPointer = System.UInt64;
global using IntPointer = System.Int64;
global using SizeT = System.Int64;
#else
#error Invalid XLink arch target.
#endif

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WoomLink.Ex
{
    public static class Arch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is64()
        {
#if XLINK_ARCH_32
            return false;
#elif XLINK_ARCH_64
            return true;
#else 
#error Invalid XLink arch target.
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is32()
        {
#if XLINK_ARCH_32
            return true;
#elif XLINK_ARCH_64
            return false;
#else 
#error Invalid XLink arch target.
#endif
        }
    }

    public struct FakeHeap
    {
        public static readonly int PointerSize = Unsafe.SizeOf<UintPointer>();

        private static UintPointer _heapPosition;

        private static Memory<byte> _heap;

        public static Pointer<char> EmptyString;

        private const int _heapSize = 0x1800000;
        private const int _baseAddress = 0x10000;
        private const int _alignmentBy = 0x1000;

        static FakeHeap()
        {
            _heap = new Memory<byte>(new byte[_heapSize]);

            EmptyString = AllocateT<char>(1);
            EmptyString.Ref = '\0';
        }
        public static UintPointer Allocate(SizeT size)
        {
            var pos = _heapPosition;
            _heapPosition += (UintPointer) size;
            _heapPosition += _alignmentBy - 1;
            _heapPosition &= ~(UintPointer)(_alignmentBy - 1);
            Debug.Assert(_heapPosition <= _heapSize);
            return pos + _baseAddress;
        }

        public static Pointer<TType> AllocateT<TType>(SizeT count) where TType : unmanaged
        {
            return Pointer<TType>.As(Allocate(count * (SizeT)Unsafe.SizeOf<TType>()));
        }

        public static Span<byte> Span(UintPointer start)
        {
            //return sead.HeapMgr.Arena!.Data.AsSpan((int)start);
            return _heap[(int)(start - _baseAddress)..].Span;
        }

        public static Span<byte> Span(UintPointer start, int length)
        {
           // return sead.HeapMgr.Arena!.Data.AsSpan((int)start, length);
            return _heap.Slice((int)(start - _baseAddress), length).Span;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public struct Pointer<TType> where TType : struct
    {
        public static int SizeT
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.SizeOf<TType>();
        }

        public static Pointer<TType> Null => new();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Pointer<TType> As(UintPointer pointer)
        {
            return new Pointer<TType>(pointer);
        }

        public UintPointer PointerValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Pointer(UintPointer pointer)
        {
            PointerValue = pointer;
        }

        public readonly ref TType Ref
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref AsSpan(1)[0];
        }

        public readonly TType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ref;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TTo> Cast<TTo>() where TTo : struct
        {
            return new Pointer<TTo>(PointerValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<TType> AsSpan(int length)
        {
            return MemoryMarshal.Cast<byte, TType>(FakeHeap.Span(PointerValue, SizeT * length));
        }

        public readonly string AsString()
        {
            var span = FakeHeap.Span(PointerValue);
            var length = 0;
            while (span[length] != 0)
                length++;
            return Encoding.UTF8.GetString(span[..length]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> Add(UintPointer count)
        {
            return As(PointerValue + count * (UintPointer)SizeT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> Add(IntPointer count)
        {
            return As(PointerValue + (UintPointer)(count * SizeT));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> AddBytes(UintPointer bytes)
        {
            return As(PointerValue + bytes);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> AddBytes(IntPointer bytes)
        {
            return As(PointerValue + (UintPointer)bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> Sub(UintPointer count)
        {
            return As(PointerValue - count * (UintPointer)SizeT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> SubBytes(UintPointer bytes)
        {
            return As(PointerValue - bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TTo> AtEnd<TTo>() where TTo : struct
        {
            return Add(1).Cast<TTo>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> AlignUp(int alignment)
        {
            return As(PointerValue + (UintPointer)(alignment - 1) & (UintPointer)~(alignment - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals<T>(Pointer<T> obj) where T : struct
        {
            return PointerValue == obj.PointerValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(UintPointer ptr)
        {
            return PointerValue == ptr;
        }

        public readonly bool IsNull
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => PointerValue == 0;
        }

        public override string ToString()
        {
            var type = GetType();
            var genericType = type.GetTypeInfo().GenericTypeArguments[0];
            if (IsNull)
            {
                return $"{genericType} (null)";
            }
            if (genericType == typeof(char))
            {
                return AsString();
            }

            return Ref.ToString()!;
        }
    }
}

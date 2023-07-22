#if !XLINK_VER_BLITZ && !XLINK_VER_THUNDER
#error Invalid XLink version target.
#endif

#if XLINK_ARCH_32
global using UintPointer = System.UInt32;
global using IntPointer = System.Int32;
#elif XLINK_ARCH_64
global using UintPointer = System.UInt64;
global using IntPointer = System.Int64;
#else
#error Invalid XLink arch target.
#endif

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WoomLink.xlink2.File
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

    public struct Heap
    {
        public static readonly int PointerSize = Unsafe.SizeOf<UintPointer>();

        private static UintPointer _heapPosition;

        private static Memory<byte> _heap;

        private const int _heapSize = 0x1800000;
        private const int _baseAddress = 0x10000;
        private const int _alignmentBy = 0x1000;

        static Heap()
        {
            _heap = new Memory<byte>(new byte[_heapSize]);
        }
        public static UintPointer Allocate(UintPointer size)
        {
            var pos = _heapPosition;
            _heapPosition += size;
            _heapPosition += _alignmentBy - 1;
            _heapPosition &= ~(UintPointer)(_alignmentBy - 1);
            Debug.Assert(_heapPosition <=_heapSize);
            return pos + _baseAddress;
        }

        public static Pointer<TType> AllocateT<TType>(UintPointer size) where TType : unmanaged
        {
            return Pointer<TType>.As(Allocate(size * (UintPointer)Unsafe.SizeOf<TType>()));
        }

        public static Span<byte> Span(UintPointer start)
        {
            return _heap[(int)(start - _baseAddress)..].Span;
        }

        public static Span<byte> Span(UintPointer start, int length)
        {
            return _heap.Slice((int)(start - _baseAddress), length).Span;
        }
    }

    [DebuggerDisplay("{ToString()}")]
    public struct Pointer<TType> where TType : unmanaged
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
        public readonly Pointer<TTo> Cast<TTo>() where TTo : unmanaged
        {
            return new Pointer<TTo>(PointerValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<TType> AsSpan(int length)
        {
            return MemoryMarshal.Cast<byte, TType>(Heap.Span(PointerValue, SizeT * length));
        }

        public readonly string AsString()
        {
            var span = Heap.Span(PointerValue);
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
        public readonly Pointer<TTo> AtEnd<TTo>() where TTo : unmanaged
        {
            return Add(1).Cast<TTo>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Pointer<TType> AlignUp(int alignment)
        {
            return As(PointerValue + ((UintPointer)(alignment - 1)) & (UintPointer)~(alignment - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsNull()
        {
            return PointerValue == 0;
        }

        public override string ToString()
        {
            var type = GetType();
            var genericType = type.GetTypeInfo().GenericTypeArguments[0];
            if (IsNull())
            {
                return $"{genericType} (null)";
            }
            if (genericType == typeof(char))
            {
                return AsString();
            }

            return Ref.ToString();
        }
    }
}

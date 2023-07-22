using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WoomLink
{
    public static class Utils
    {
        public static Span<byte> AsSpan<T>(ref T val) where T : unmanaged
        {
            Span<T> valSpan = MemoryMarshal.CreateSpan(ref val, 1);
            return MemoryMarshal.Cast<T, byte>(valSpan);
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo dir, string sub)
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, sub));
        }

        public static FileInfo GetFile(this DirectoryInfo dir, string name)
        {
            return new FileInfo(Path.Combine(dir.FullName, name));
        }

        public static Span<T> Slice<T>(this Span<T> span, Range range)
        {
            return span[range.Start.Value..range.End.Value];
        }

        public static int Length(this Range range)
        {
            return range.End.Value - range.Start.Value;
        }

        public static void FlipKeyEndianness(this byte[] key)
        {
            /* Ensure the length is divisible by sizeof(uint) */
            if (key.Length % 4 != 0)
                return;

            /* Flip every 32-bit value. */
            for (int i = 0; i < key.Length / 4; i++)
                Array.Reverse(key, i * 4, 4);
        }

        public static TemporarySeekHandle TemporarySeek(this Stream stream)
        {
            return stream.TemporarySeek(0, SeekOrigin.Current);
        }

        public static TemporarySeekHandle TemporarySeek(this Stream stream, long offset, SeekOrigin origin)
        {
            long ret = stream.Position;
            stream.Seek(offset, origin);
            return new TemporarySeekHandle(stream, ret);
        }

        public static uint ReadUInt24(this BinaryReader reader)
        {
            uint v = reader.ReadUInt32();

            /* Mask out upper byte. */
            v &= ~(0xFF << 24);

            /* Seek back one byte. */
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return v;
        }

        public static int BinarySearch<T, K>(IList<T> arr, K v) where T : IComparable<K>
        {
            var start = 0;
            var end = arr.Count - 1;

            while (start <= end)
            {
                var mid = (start + end) / 2;
                var entry = arr[mid];
                var cmp = entry.CompareTo(v);

                if (cmp == 0)
                    return mid;
                if (cmp > 0)
                    end = mid - 1;
                else /* if (cmp < 0) */
                    start = mid + 1;
            }

            return ~start;
        }

        public static int BinarySearch<T, K>(ReadOnlySpan<T> arr, K v) where T : IComparable<K>
        {
            var start = 0;
            var end = arr.Length - 1;

            while (start <= end)
            {
                var mid = (start + end) / 2;
                var entry = arr[mid];
                var cmp = entry.CompareTo(v);

                if (cmp == 0)
                    return mid;
                if (cmp > 0)
                    end = mid - 1;
                else /* if (cmp < 0) */
                    start = mid + 1;
            }

            return ~start;
        }

        public static int BinarySearch<T>(ReadOnlySpan<T> arr, Func<T, int> callback)
        {
            var start = 0;
            var end = arr.Length - 1;

            while (start <= end)
            {
                var mid = (start + end) / 2;
                var entry = arr[mid];
                var cmp = callback(entry);

                if (cmp == 0)
                    return mid;
                if (cmp > 0)
                    end = mid - 1;
                else /* if (cmp < 0) */
                    start = mid + 1;
            }

            return ~start;
        }

        public static T[] ReadArray<T>(this Stream stream, uint count) where T : struct
        {
            /* Allocate space for data. */
            T[] data = new T[count];

            /* Read into casted span. */
            stream.Read(MemoryMarshal.Cast<T, byte>(data));

            return data;
        }

        public static void WriteAll(this FileInfo info, Span<byte> data)
        {
            using var stream = info.OpenWrite();
            stream.Position = 0;
            stream.Write(data);
        }

        public static void BufferedReadAll(this Stream stream, long bufferSize, Action<byte[]> callback)
        {
            var size = stream.Length;

            /* Stream data into callback. */
            byte[] buffer = new byte[bufferSize];
            for (int i = 0; i < size / bufferSize; i++)
            {
                stream.Read(buffer);
                callback(buffer);
            }

            /* Also get any trailing data when file isn't divisible by bufferSize. */
            var trailingSize = size % bufferSize;
            if (trailingSize > 0)
            {
                buffer = new byte[trailingSize];
                stream.Read(buffer);
                callback(buffer);
            }
        }

        public static string ReadUtf8(this BinaryReader reader, int size)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(size), 0, size);
        }

        public static string ReadUtf8Z(this BinaryReader reader, int maxLength = int.MaxValue)
        {
            long start = reader.BaseStream.Position;
            int size = 0;

            // Read until we hit the end of the stream (-1) or a zero
            while (reader.BaseStream.ReadByte() - 1 > 0 && size < maxLength)
            {
                size++;
            }

            reader.BaseStream.Position = start;
            string text = reader.ReadUtf8(size);
            reader.BaseStream.Position++; // Skip the null byte

            if (text.Contains("LobbyLocal_"))
                ;

            return text;
        }

        public static string GetNullTermString(this string str, ulong start)
        {
            ulong end = (ulong)str.IndexOf('\0', (int)start);
            return str.Substring((int)start, (int)(end - start));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BitCount(this ulong i)
        {
            i = i - ((i >> 1) & 0x5555555555555555UL);
            i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
            return (uint)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AlignUp(uint num, uint align)
        {
            return (num + (align - 1)) & ~(align - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignUp(ulong num, ulong align)
        {
            return (num + (align - 1)) & ~(align - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AlignUp(long num, long align)
        {
            return (num + (align - 1)) & ~(align - 1);
        }


        public static void MaybeAdjustEndianness<T>(Type type, Span<T> data, Endianness endianness) where T : struct
        {
            for(var i = 0; i < data.Length; i++)
                MaybeAdjustEndianness(type, ref data[i], endianness);
        }


        public static void MaybeAdjustEndianness<T>(Type type, ref T data, Endianness endianness) where T : struct
        {
            MaybeAdjustEndiannessRaw(type, MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref data, Unsafe.SizeOf<T>())), endianness);
        }

        public static void MaybeAdjustEndiannessRaw(Type type, Span<byte> data, Endianness endianness, int startOffset = 0)
        {
            if (Endian.Native == endianness)
            {
                // nothing to change => return
                return;
            }

            foreach (var field in type.GetFields())
            {
                var fieldType = field.FieldType;
                if (field.IsStatic)
                    // don't process static fields
                    continue;

                if (fieldType == typeof(string))
                    // don't swap bytes for strings
                    continue;

                var offset = Marshal.OffsetOf(type, field.Name).ToInt32();

                // handle enums
                if (fieldType.IsEnum)
                    fieldType = Enum.GetUnderlyingType(fieldType);

                // check for sub-fields to recurse if necessary
                var subFields = fieldType.GetFields().Where(subField => subField.IsStatic == false).ToArray();

                var effectiveOffset = startOffset + offset;

                if (subFields.Length == 0)
                {
                    var r = data.Slice(effectiveOffset, Marshal.SizeOf(fieldType));
                    r.Reverse();
                }
                else
                {
                    // recurse
                    MaybeAdjustEndiannessRaw(fieldType, data, endianness, effectiveOffset);
                }
            }
        }

        public static int ParseInt(string s)
        {
            if (s.StartsWith("0x"))
            {
                return int.Parse(s[2..], NumberStyles.HexNumber);
            }
            else
            {
                return int.Parse(s);
            }
        }
    }
}

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WoomLink.Ex.sead
{
    public class Yaz0
    {
        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        public struct Yaz0Header
        {
            public uint Magic;
            public uint DecompressedSize;
            public uint DecompressedAlignment;
            public uint Reserved;
        }

        /* Yaz0 backwards, because it's big endian. */
        private const uint Magic = 0x307A6159;

        private static void DecompressToImpl(Stream stream, Span<byte> output, ref Yaz0Header header)
        {
            if (header.Magic != Magic)
                throw new Exception("Bad Yaz0 magic!");

            /* Reverse because it's big endian. */
            header.DecompressedSize = header.DecompressedSize.ByteReversed();
            header.DecompressedAlignment = header.DecompressedAlignment.ByteReversed();

            BinaryReader inputReader = new(stream);
            var bodySize = stream.Length - Unsafe.SizeOf<Yaz0Header>();
            var dst = 0;
            var groupHeader = 0;
            var chunksLeft = 0;

            while (inputReader.BaseStream.Position < bodySize && dst < header.DecompressedSize)
            {
                if (chunksLeft == 0)
                {
                    groupHeader = inputReader.ReadByte();
                    chunksLeft = 8;
                }

                if ((groupHeader & 0x80) == 0x80)
                {
                    output[dst++] = inputReader.ReadByte();
                }
                else
                {
                    var pair = inputReader.ReadUInt16().ByteReversed();

                    var distance = (pair & 0x0FFF) + 1;
                    var length = ((pair >> 12) != 0 ? (pair >> 12) : (inputReader.ReadByte() + 16)) + 2;
                    var b = dst - distance;

                    if (b < 0 || dst + length > header.DecompressedSize)
                        throw new InvalidDataException("Corrupt data!");

                    while (length-- > 0)
                        output[dst++] = output[b++];
                }

                groupHeader <<= 1;
                chunksLeft--;
            }
        }

        public static void DecompressTo(Stream stream, Span<byte> output)
        {
            Yaz0Header header = new();
            stream.Read(Utils.AsSpan(ref header));
            DecompressToImpl(stream, output, ref header);
        }

        public static byte[] Decompress(Stream stream)
        {
            Yaz0Header header = new();
            using (stream.TemporarySeek())
            {
                stream.Read(Utils.AsSpan(ref header));
            }

            var output = new byte[header.DecompressedSize.ByteReversed()];
            DecompressTo(stream, output);
            return output;
        }


        public static bool IsYaz0(Stream storage)
        {
            Yaz0Header header = new();
            using (storage.TemporarySeek())
            {
                storage.Read(Utils.AsSpan(ref header));
            }
            return header.Magic == Magic;
        }
    }
}

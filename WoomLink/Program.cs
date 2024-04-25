using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using WoomLink.Ex;
using WoomLink.Ex.sead;
using WoomLink.sead;
using WoomLink.xlink2;
using WoomLink.xlink2.File;
using WoomLink.xlink2.Properties;

namespace WoomLink
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Default;

            var edata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\Splatoon 3\7.1.0\Program\Data\ELink2\elink2.Product.710.belnk.zs"));
            var sdata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\Splatoon 3\7.1.0\Program\Data\SLink2\slink2.Product.710.bslnk.zs"));

            // var edata = LoadYaz0SarcFileOntoHeap(new(@"R:\Games\Splatoon 2 Global Testfire\1.0.0 (Base)\Program\Data\ELink2\ELink2DB.szs"));
            // var sdata = LoadYaz0SarcFileOntoHeap(new(@"R:\Games\Splatoon 2 Global Testfire\1.0.0 (Base)\Program\Data\SLink2\SLink2DB.szs"));

            // var edata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\Mario vs. Donkey Kong™ Demo\1.0.0 (Base)\Program\Data\ELink2\elink2.Product.dmo.belnk.zs"));
            // var sdata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\Mario vs. Donkey Kong™ Demo\1.0.0 (Base)\Program\Data\SLink2\slink2.Product.dmo.bslnk.zs"));

            // var dicts = LoadZstdCompressedData(new(@"R:\Games\The Legend of Zelda Tears of the Kingdom\1.0.0 (Base)\Program\Data\Pack\ZsDic.pack.zs"));
            // var dictsSarc = new Sarc(dicts);
            // var dict = dictsSarc.OpenFile(dictsSarc.GetNodeIndex("zs.zsdic")).ToArray();
            // var edata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\The Legend of Zelda Tears of the Kingdom\1.0.0 (Base)\Program\Data\ELink2\elink2.Product.100.belnk.zs"), dict);
            // var sdata = LoadZstdCompressedDataOntoHeap(new(@"R:\Games\The Legend of Zelda Tears of the Kingdom\1.0.0 (Base)\Program\Data\SLink2\slink2.Product.100.bslnk.zs"), dict);

            var esystem = SystemELink.GetInstance();
            var ssystem = SystemSLink.GetInstance();
            const int eventPoolNum = 96;
            esystem.Initialize(null, eventPoolNum);
            ssystem.Initialize(eventPoolNum);

            void SetupSystem(xlink2.System system, Pointer<byte> resource, PropertyDefinition[] globalProp)
            {
                var r = system.LoadResource(resource.PointerValue);
                Debug.Assert(r);
                system.AllocGlobalProperty((uint)globalProp.Length);
                for (uint i = 0; i < globalProp.Length; i++)
                {
                    system.SetGlobalPropertyDefinition(i, globalProp[i]);
                }
                system.FixGlobalPropertyDefinition();
            }
            //var globProp = PropertyParser.Parse(new FileInfo(@"C:\Users\shado\Downloads\blitz-globprop.txt").OpenRead());
            var globProp = Array.Empty<PropertyDefinition>();
            SetupSystem(esystem, edata, globProp);
            SetupSystem(ssystem, sdata, globProp);

            Console.WriteLine(PrintUserByName(esystem, "Player"));


            PrintAllUsers(esystem);
            PrintAllUsers(ssystem);
        }

        private static string? PrintUserByIndex(xlink2.System system, int index)
        {
            ref var param = ref system.ResourceBuffer.RSP;
            var user = param.UserDataPointersSpan[index];

            ResourceParamCreator.CreateUserBinParam(out var userParam, user, in system.GetParamDefineTable());
            var writer = new UserPrinter();
            writer.Print(system, in param.Common, in userParam);
            return writer.Writer.ToString()!;
        }

        private static string? PrintUserByName(xlink2.System system, string name)
        {
            ref var param = ref system.ResourceBuffer.RSP;

            var idx = Utils.BinarySearch<uint, uint>(param.UserDataHashesSpan, HashCrc32.CalcStringHash(name));
            if (idx < 0)
                return null;

            return PrintUserByIndex(system, idx);
        }

        private static void SaveUsersFromNames(xlink2.System system, StreamReader reader, DirectoryInfo outDir)
        {
            outDir.Create();
            string userName;
            while ((userName = reader.ReadLine()) != null)
            {
                var text = PrintUserByName(system, userName);
                if (text == null)
                {
                    Console.WriteLine($"Failed to read {userName}");
                    continue;
                }

                var outFi = outDir.GetFile($"{userName}.txt");
                File.WriteAllText(outFi.FullName, text, Encoding.UTF8);
            }
        }

        private static void PrintAllUsers(xlink2.System system)
        {
            ref var param = ref system.ResourceBuffer.RSP;
            for (var i = 0; i < param.NumUser; i++)
            {
                var name = param.UserDataHashesSpan[i];
                Console.WriteLine($"{name:X8}");
                Console.WriteLine(PrintUserByIndex(system, i));
            }
        }

        private static Pointer<byte> LoadRawDataOntoHeap(FileInfo info)
        {
            using var stream = info.OpenRead();
            var ptr = Ex.FakeHeap.AllocateT<byte>((SizeT)stream.Length);
            stream.Read(ptr.AsSpan((int)stream.Length));
            return ptr;
        }

        private static T LoadZstdCompressedDataTo<T>(FileInfo info, byte[]? dict, Func<Stream, SizeT, T> callback)
        {
            const int ZSTD_frameHeaderSize_max = 18;
            var frameHeader = new byte[ZSTD_frameHeaderSize_max];
            using var stream = info.OpenRead();
            stream.Read(frameHeader);
            stream.Position = 0;

            Stream decompressStream;
            if (dict != null)
            {
                decompressStream = new ZstdNet.DecompressionStream(stream, new ZstdNet.DecompressionOptions(dict));
            }
            else
            {
                decompressStream = new ZstdNet.DecompressionStream(stream);
            }

            using (decompressStream)
            {
                var decompressedSize = ZstdNet.Decompressor.GetDecompressedSize(frameHeader);
                return callback(decompressStream, (SizeT)decompressedSize);
            }
        }

        private static Pointer<byte> LoadZstdCompressedDataOntoHeap(FileInfo info, byte[]? dict = null)
        {
            return LoadZstdCompressedDataTo(info, dict, (stream, length) =>
            {
                var ptr = FakeHeap.AllocateT<byte>(length);
                stream.Read(ptr.AsSpan((int)length));
                return ptr;
            });
        }

        private static byte[] LoadZstdCompressedData(FileInfo info, byte[]? dict = null)
        {
            return LoadZstdCompressedDataTo(info, dict, (stream, length) =>
            {
                var bytes = new byte[length];
                stream.Read(bytes);
                return bytes;
            });
        }

        private static byte[] LoadYaz0CompressedData(FileInfo info)
        {
            using var stream = info.OpenRead();
            return Yaz0.Decompress(stream);
        }

        private static Span<byte> LoadYaz0SarcFile(FileInfo info)
        {
            var decompressed = LoadYaz0CompressedData(info);
            var sarc = new Sarc(decompressed);
            if (sarc.FileNodes.Length != 1)
                throw new Exception("Invalid file count in SARC!");

            return sarc.OpenFile(0);
        }

        private static Pointer<byte> LoadYaz0SarcFileOntoHeap(FileInfo info)
        {
            var data = LoadYaz0SarcFile(info);
            var ptr = FakeHeap.AllocateT<byte>(data.Length);
            data.CopyTo(ptr.AsSpan(data.Length));
            return ptr;
        }

        private static Pointer<byte> LoadYaz0FileOntoHeap(FileInfo info)
        {
            using var stream = info.OpenRead();
            if (!Yaz0.IsYaz0(stream))
                throw new Exception("Not Yaz0!");

            Yaz0.Yaz0Header header = new();
            using (stream.TemporarySeek())
            {
                stream.Read(Utils.AsSpan(ref header));
            }

            var ptr = FakeHeap.AllocateT<byte>((SizeT)header.DecompressedSize.ByteReversed());
            Yaz0.DecompressTo(stream, ptr.AsSpan((int)header.DecompressedSize.ByteReversed()));
            return ptr;
        }
    }
}

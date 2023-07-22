using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WoomLink.Ex;
using WoomLink.sead;
using WoomLink.xlink2;
using WoomLink.xlink2.File;
using WoomLink.xlink2.Properties;
using WoomLink.xlink2.Properties.Enum;

namespace WoomLink
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Default;

            var epath = @"Z:\Switch\Games\The Legend of Zelda Breath of the Wild\1.6.0\romfs\Pack\ELink2DB.belnk";
            var spath = @"Z:\Switch\Games\The Legend of Zelda Breath of the Wild\1.6.0\romfs\Pack\SLink2DB.bslnk";

            Pointer<byte> LoadData(FileInfo info)
            {
                using var stream = info.OpenRead();
                var ptr = Heap.AllocateT<byte>((UintPointer)stream.Length);
                stream.Read(ptr.AsSpan((int)stream.Length));
                return ptr;
            }

            var edata = LoadData(new(epath));
            var sdata = LoadData(new(spath));

            var esystem = SystemELink.GetInstance();
            var ssystem = SystemSLink.GetInstance();
            const int eventPoolNum = 96;
            esystem.Initialize(eventPoolNum);
            ssystem.Initialize(eventPoolNum);

            void SetupSystem(xlink2.System system, Pointer<byte> resource, PropertyDefinition[] globalProp)
            {
                system.LoadResource(resource.PointerValue);
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

            void PrintUser(xlink2.System system, string name)
            {
                var param = system.ResourceBuffer.RSP;

                var idx = Utils.BinarySearch<uint, uint>(param.UserDataHashesSpan, HashCrc32.CalcStringHash(name));
                var user = param.UserDataPointersSpan[idx];

                ResourceParamCreator.CreateUserBinParam(out var userParam, user, in system.GetParamDefineTable());
                new UserPrinter().Print(system, ref system.ResourceBuffer.RSP.Common, ref userParam);
                var s = UserPrinter._writer.ToString();
                Console.WriteLine(s);
            }

            PrintUser(ssystem, "Player");
        }
    }
}

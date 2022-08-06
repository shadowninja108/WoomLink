using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WoomLink.Ex;
using WoomLink.sead;
using WoomLink.xlink2;

namespace WoomLink
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HashDb.Load(new FileInfo(@"C:\Users\shado\Downloads\xlink-uniq.txt"));

            var globProp = PropertyParser.Parse(new FileInfo(@"C:\Users\shado\Downloads\blitz-globprop.txt").OpenRead());

            string epath = @"C:\Users\shado\Downloads\ELink2DB.belnk";
            string spath = @"C:\Users\shado\Downloads\SLink2DB.bslnk";

            FileInfo efi = new(epath);
            FileInfo sfi = new(spath);

            using var estream = efi.OpenRead();
            using var sstream = sfi.OpenRead();

            SystemELink esystem = new();
            SystemSLink ssystem = new();

            esystem.LoadResource(estream);
            ssystem.LoadResource(sstream);
            
            esystem.GlobalPropertyDefinitions = globProp;
            ssystem.GlobalPropertyDefinitions = globProp;

            esystem.FixGlobalPropertyDefinition();
            ssystem.FixGlobalPropertyDefinition();
        }
    }
}

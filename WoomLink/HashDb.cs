using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WoomLink.sead;

namespace WoomLink
{
    public class HashDb
    {
        private static readonly List<uint> Hashes = new();
        private static readonly List<string> Values = new();

        public static bool Load(FileInfo file)
        {
            try
            {
                return LoadImpl(file);
            }
            catch
            {
                return false;
            }
        }

        private static bool LoadImpl(FileInfo file)
        {
            using var stream = file.OpenRead();
            using var reader = new StreamReader(stream);

            var lines = File.ReadLines(file.FullName);

            foreach (var line in lines)
            {
                string[] split = line.Split(": ");

                var value = split[1];
                var hash = HashCrc32.CalcStringHash(value);

                if (!TryAddHash(value, hash))
                {
                    // Console.WriteLine("Malformed line! (Duplicate line)");
                    continue;
                }
            }
            return true;
        }

        public static bool TryAddHash(string value, uint hash)
        {
            var idx = Utils.BinarySearch(Hashes, hash);

            if (idx >= 0)
                return false;

            idx = ~idx;

            Hashes.Insert(idx, hash);
            Values.Insert(idx, value);
            return true;
        }

        public static string? FindByHash(uint hash)
        {

            int idx = Utils.BinarySearch(Hashes, hash);
            if (idx < 0)
                return null;
            return Values[idx];
        }
    }
}

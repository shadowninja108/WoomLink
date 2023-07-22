
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink
{
    public class Endian
    {
        public static Endianness Native =>
            BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

        public static Endianness OppositeOfNative => Opposite(Native);

        public static Endianness Opposite(Endianness v) => v == Endianness.Little ? Endianness.Big : Endianness.Little;
    }
}

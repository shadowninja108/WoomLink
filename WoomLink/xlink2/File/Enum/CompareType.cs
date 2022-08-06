using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Enum
{
    public enum CompareType : uint
    {
        Equal = 0x0,
        GreaterThan = 0x1,
        GreaterThanOrEqual = 0x2,
        LessThan = 0x3,
        LessThanOrEqual = 0x4,
        NotEqual = 0x5,
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResCurveCallTableEx
    {
        public ResCurveCallTable Internal;

        public string PropName;

        public ResCurveCallTableEx(ResCurveCallTable @internal)
        {
            Internal = @internal;
        }

        public void Solve(Stream stream, CommonResourceParam param)
        {
            using (stream.TemporarySeek(param.NameTablePos + Internal.PropName, SeekOrigin.Begin))
                PropName = new BinaryReader(stream).ReadUtf8Z();
        }
    }
}

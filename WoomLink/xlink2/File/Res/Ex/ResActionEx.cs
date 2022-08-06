using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResActionEx
    {
        public ResAction Internal;
        public string Name;

        public ResActionEx(ResAction @internal)
        {
            Internal = @internal;
        }

        public void Solve(Stream stream, CommonResourceParam param)
        {
            using (stream.TemporarySeek(param.NameTablePos + Internal.Name, SeekOrigin.Begin))
                Name = new BinaryReader(stream).ReadUtf8Z();
        }
    }
}

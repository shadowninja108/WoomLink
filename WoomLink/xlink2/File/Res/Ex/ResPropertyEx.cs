using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResPropertyEx
    {
        public ResProperty Internal;
        public string WatchPropertyName;

        public ResPropertyEx(ResProperty @internal)
        {
            Internal = @internal;
        }

        public void Solve(Stream stream, CommonResourceParam param)
        {
            using (stream.TemporarySeek(param.NameTablePos + Internal.WatchPropertyNamePos, SeekOrigin.Begin))
                WatchPropertyName = new BinaryReader(stream).ReadUtf8Z();
        }
    }
}

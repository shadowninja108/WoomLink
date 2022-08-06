using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.File.Res.Ex
{
    public class ResContainerParamEx
    {
        public ResContainerParam Internal;

        [StructLayout(LayoutKind.Sequential, Size = 0xC)]
        public struct ForSwitch
        {
            public uint WatchPropertyNamePos;
            public int WatchPropertyId;
            public short LocalPropertyNameIdx;
            public bool IsGlobal;
        }

        public ForSwitch SwitchValue;
        public string WatchPropertyName;

        public bool IsSwitch => Internal.Type == ContainerType.Switch;

        public ResContainerParamEx(Stream stream)
        {
            stream.Read(Utils.AsSpan(ref Internal));

            if (IsSwitch)
                stream.Read(Utils.AsSpan(ref SwitchValue));
        }

        public void Solve(Stream stream, CommonResourceParam param)
        {
            if (!IsSwitch)
                return;

            using (stream.TemporarySeek(param.NameTablePos + SwitchValue.WatchPropertyNamePos, SeekOrigin.Begin))
                WatchPropertyName = new BinaryReader(stream).ReadUtf8Z();
        }

        public static IEnumerable<ResContainerParamEx> Read(Stream stream, uint num)
        {
            for (int i = 0; i < num; i++)
                yield return new ResContainerParamEx(stream);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public class ParamDefineTable
    {
        public class ParamDefineEx
        {
            private ParamDefine Internal;

            public ParamDefineEx(ParamDefine @internal, string stringTable)
            {
                Internal = @internal;

                Name = stringTable.GetNullTermString(Internal.Name);

                if(Type == ParamType.String)
                {
                    DefaultValueAsString = stringTable.GetNullTermString((uint)Internal.DefaultValue);
                }
            }

            public string Name {  get; set; }
            public ParamType Type => Internal.Type;
            public int DefaultValue => Internal.DefaultValue;

            public float DefaultValueAsFloat => BitConverter.Int32BitsToSingle(Internal.DefaultValue);
            public string DefaultValueAsString = null;
        }

        public ParamDefineTableHeader Header;

        public ParamDefineEx[] UserParam;
        public ParamDefineEx[] AssetParam;
        public ParamDefineEx[] TriggerParam;

        private string StringTable;

        public void Setup(Stream stream, int userParamNum)
        {
            var start = stream.Position;
            stream.Read(Utils.AsSpan(ref Header));

            var userParam = stream.ReadArray<ParamDefine>(Header.NumUserParams);
            var assetParam = stream.ReadArray<ParamDefine>(Header.NumAssetParams);
            var triggerParam = stream.ReadArray<ParamDefine>(Header.NumTriggerParams);

            /* Rest of data is string table. */
            byte[] stringTableRaw = new byte[Header.Size - (stream.Position - start)];
            stream.Read(stringTableRaw);

            StringTable = Encoding.UTF8.GetString(stringTableRaw);

            UserParam = BuildParamDefineExArray(userParam);
            AssetParam = BuildParamDefineExArray(assetParam);
            TriggerParam = BuildParamDefineExArray(triggerParam);
        }

        private ParamDefineEx[] BuildParamDefineExArray(ParamDefine[] raw)
        {
            return raw.Select((r) => new ParamDefineEx(r, StringTable)).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public class ResourceBuffer
    {
        private const uint Magic = 0x4B4E4C58; /* XLNK */

        public ParamDefineTable PDT = new();
        public RomResourceParam RSP = new();

        public bool Load(Stream stream, System system)
        {
            ResourceHeader header = new();
            stream.Read(Utils.AsSpan(ref header));

            /* Ensure magic matches. */
            if (header.Magic != Magic)
                return false;

            /* Ensure version matches. */
            if (header.Version != system.GetResourceVersion())
                return false;

            /* Skip user offset/hashes.  */
            stream.Position += (header.NumUser * Unsafe.SizeOf<uint>()) * 2;

            PDT.Setup(stream, system.GetUserParamNum());

            /* Move back to the start. */
            stream.Position = 0;

            ResourceParamCreator.CreateParamAndSolveResource(RSP, stream, PDT, system);

            return true;
        }

        public void ApplyGlobalPropertyDefinition(System system)
        {
            /* field_94? */
            ResourceParamCreator.SolveAboutGlobalProperty(RSP, PDT, system);
        }
    }
}

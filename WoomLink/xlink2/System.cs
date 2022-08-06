using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoomLink.xlink2.File;
using WoomLink.xlink2.Properties;

namespace WoomLink.xlink2
{
    public abstract class System
    {
        public ResourceBuffer ResourceBuffer = new();
        public PropertyDefinition[] GlobalPropertyDefinitions { get; set; }

        public bool LoadResource(Stream stream)
        {
            /* Don't bother with stupid high/low address here. */
            return ResourceBuffer.Load(stream, this);
        }

        public void FixGlobalPropertyDefinition()
        {
            ResourceBuffer.ApplyGlobalPropertyDefinition(this);
        }

        public int SearchGlobalPropertyIndex(string name)
        {
            for (var i = 0; i < GlobalPropertyDefinitions.Length; i++)
            {
                var def = GlobalPropertyDefinitions[i];
                if (def.Name == name)
                    return i;
            }

            return -1;
        }

        public abstract int GetUserParamNum();
        public abstract int GetResourceVersion();
    }
}

using System.Collections.Generic;
using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.Properties.Enum
{
    public class EnumPropertyDefinition : PropertyDefinition
    {
        public override PropertyType Type => PropertyType.Enum;

        public string TypeName;
        public IList<EnumDefinition> Enums;

        public int SearchEntryValueByKey(string key)
        {
            for (var i = 0; i < Enums.Count; i++)
            {
                var enu = Enums[i];
                if (enu.Name == key)
                    return i;
            }

            return -1;
        }
    }
}

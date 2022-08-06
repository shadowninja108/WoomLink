using WoomLink.xlink2.File.Enum;

namespace WoomLink.xlink2.Properties
{
    public abstract class PropertyDefinition
    {
        public string Name;

        public abstract PropertyType Type
        {
            get;
        }
    }
}

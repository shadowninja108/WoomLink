using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public class UserResourceParam
    {
        public CommonResourceParam? Common;
        public UserBinParam User;
        public sbyte[] PropertyNameIndexToPropertyIndex;
        public ResCallTable[] CallTables;
        public bool[] ActionTriggers;
        public ulong PropertyAssignedBitfield;
        public ResourceAccessor Accessor;
        public bool Setup = false;
    }
}

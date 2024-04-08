using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2.User.Resource
{
    public class UserResourceParam
    {
        /* This is normally its own pointer, but this was easier to arrange. */
        public ref CommonResourceParam Common => ref Accessor.System.ResourceBuffer.RSP.Common;
        public UserBinParam User;
        public sbyte[] PropertyNameIndexToPropertyIndex;
        public ResCallTable[] CallTables;
        public bool[] ActionTriggers;
        public ulong PropertyAssignedBitfield;
        public ResourceAccessor Accessor;
        public bool Setup = false;
    }
}

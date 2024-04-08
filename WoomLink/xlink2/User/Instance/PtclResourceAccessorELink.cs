using System;
using WoomLink.Ex;
using WoomLink.sead.ptcl;

namespace WoomLink.xlink2.User.Instance
{
    public class PtclResourceAccessorELink
    {
        public bool SearchEmitterSetID(PtclSystem pctlSystem, Pointer<char> name, out int resourceIndex, out int emitterId)
        {
            resourceIndex = -1;
            emitterId = -1;
            throw new NotImplementedException();
        }
    }
}

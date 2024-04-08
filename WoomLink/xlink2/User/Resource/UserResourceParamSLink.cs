using System;
using WoomLink.Ex;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.User.Resource
{
    public class UserResourceParamSLink : UserResourceParam
    {
        public Pointer<char> GroupName;
        public Pointer<char> DistanceParamSetName;
        public int LimitType;
        public int PlayableLimitNum;
        public float Priority;
        public bool NotPositioned;
        public float DopplerFactor;
        public int ArrangeGroupParamsCount;
        public Pointer<ArrangeGroupParam> ArrangeGroupParamsPointer;

        public Span<ArrangeGroupParam> ArrangeGroupParams
        {
            get
            {
                if (ArrangeGroupParamsCount == 0 || ArrangeGroupParamsPointer.IsNull)
                    return [];

                return ArrangeGroupParamsPointer.AsSpan(ArrangeGroupParamsCount);
            }
        }
    }
}

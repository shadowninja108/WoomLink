namespace WoomLink.xlink2
{
    public class UserResourceParamELink : UserResourceParam
    {
        public struct ResEsetGroupIdPair
        {
            public ResEset EsetVal;
            public byte GroupId;
        }

        public ResEsetGroupIdPair[] SolvedAssetParameterByAssetId;
    }
}

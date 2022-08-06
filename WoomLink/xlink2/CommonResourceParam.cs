using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Res.Ex;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2
{
    public class CommonResourceParam
    {
        public ResourceHeader Header;
        public long NameTablePos;
        public long ConditionTablePos;
        public long TriggerOverwriteParamsPos;

        public uint[] UserDataHashes;
        public uint[] UserBinOffsets;

        public long AssetParamsPos;
        public ResAssetParam[] AssetParams;
        public ResTriggerOverwriteParam[] TriggerOverwriteParams;
        public string[] LocalPropertyNameTable;
        public string[] LocalPropertyEnumNameTable;
        public uint[] DirectValueTable;
        public ResRandomCallTable[] RandomTable;
        public ResCurveCallTableEx[] CurveCallTable;
        public CurvePointTable[] CurvePointTable;
        public ResCondition[] ConditionTable;

        public UserBinParam[] UserBinParams;
    }
}

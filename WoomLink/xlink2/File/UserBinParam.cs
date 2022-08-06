using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Res.Ex;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public class UserBinParam
    {
        public string Key;

        public ResUserHeader Header;
        public long ContainerParamTablePos;
        public long AssetCallTablePos;

        public LocalPropertyRef[] LocalPropertyRefTable;
        public ResParamEx[] UserParamTable;
        public ushort[] SortedAssetIdTable;
        public ResAssetCallTable[] AssetCallTable;
        public ResContainerParamEx[] ContainerParamTable;

        public ResActionSlot[] ResActionSlotTable;
        public ResAction[] ResActionTable;
        public ResActionTrigger[] ResActionTriggerTable;
        public ResProperty[] ResPropertyTable;
        public ResPropertyTrigger[] ResPropertyTriggerTable;
        public ResAlwaysTrigger[] ResAlwaysTriggerTable;

        public string[] LocalPropertyStringTable;
        public ResAssetCallTableEx[] AssetCallTableEx;
        public ResActionSlotEx[] ResActionSlotTableEx;
        public ResActionEx[] ResActionTableEx;
        public ResPropertyEx[] ResPropertyTableEx;
        public ResPropertyTriggerEx[] ResPropertyTriggerTableEx;
        public ResActionTriggerEx[] ResActionTriggerTableEx;
    }
}

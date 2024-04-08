using WoomLink.Ex;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public struct Locator()
    {
        private static int DataLoadedCount;

        public Pointer<ResAssetCallTable> Act = Pointer<ResAssetCallTable>.Null;
        public byte Flags = 0;
        public TriggerType TriggerType { get; private set; } = TriggerType.None;

        public void Reset()
        {
            Act = Pointer<ResAssetCallTable>.Null;
            Flags = 0;
        }

        /* Following functions appear to be stubbed in prod, maybe dead code. */
        public void SetTriggerInfo(TriggerType type, Pointer<ResTriggerOverwriteParam> overwriteParam) { }
        public Pointer<ResTriggerOverwriteParam> GetTriggerOverwriteParam() => Pointer<ResTriggerOverwriteParam>.Null;
        /* Returns xlink2::BoneMtx. */
        public void GetOverwriteBoneMtx() {}
        public bool IsNeedRebind() => false;
        /* Unknown return type */
        public void GetSearchedGuid() { }

        public static void UpdateDataLoadedCount()
        {
            DataLoadedCount++;
        }
    }
}

namespace WoomLink.xlink2.File.Res
{
    public struct ResProperty
    {
        public Pointer<char> WatchPropertyNamePos;
        public uint IsGlobal;
        public uint TriggerStartIdx;
        public uint TriggerEndIdx;

        public bool IsGlobalBool => IsGlobal != 0;
    }
}

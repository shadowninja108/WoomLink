namespace WoomLink.xlink2
{
    public struct ResEset
    {
        public short ResourceIndex;
        public short EmitterId;

        public ResEset()
        {
            ResourceIndex = -1;
            EmitterId = -1;
        }

        public readonly bool IsInvalid => ResourceIndex == -1 && EmitterId == -1;
    }
}

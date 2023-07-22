namespace WoomLink.xlink2.File.Enum
{
    public enum CompareType :
#if XLINK_VER_BLITZ
        uint
#elif XLINK_VER_THUNDER
        byte
#else
#error Invalid XLink target.
#endif
    {
        Equal = 0x0,
        GreaterThan = 0x1,
        GreaterThanOrEqual = 0x2,
        LessThan = 0x3,
        LessThanOrEqual = 0x4,
        NotEqual = 0x5,
    }
}

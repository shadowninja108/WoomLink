namespace WoomLink.xlink2.File.Enum
{
    public enum CompareType :
#if XLINK_VER_BLITZ
        uint
#elif XLINK_VER_PARK || XLINK_VER_THUNDER || XLINK_VER_EXKING
        byte
#else
#error Invalid XLink version target.
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

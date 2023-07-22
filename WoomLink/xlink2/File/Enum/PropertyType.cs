namespace WoomLink.xlink2.File.Enum
{
    public enum PropertyType :
#if XLINK_VER_BLITZ
        uint
#elif XLINK_VER_THUNDER
        byte
#else
#error Invalid XLink target.
#endif
    {
        Enum = 0x0,
        S32 = 0x1,
        F32 = 0x2,
        End = 0x3
    }
}

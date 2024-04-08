namespace WoomLink.xlink2.File.Enum
{
    public enum PropertyType :
#if XLINK_VER_BLITZ
        uint
#elif XLINK_VER_PARK || XLINK_VER_THUNDER || XLINK_VER_EXKING
        byte
#else
#error Invalid XLink version target.
#endif
    {
        Enum = 0x0,
        S32 = 0x1,
        F32 = 0x2,
        /* TODO: there might be more in ExKing (maybe Thunder?). */
        End = 0x3
    }
}

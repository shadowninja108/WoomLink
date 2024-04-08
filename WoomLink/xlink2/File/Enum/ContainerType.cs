namespace WoomLink.xlink2.File.Enum
{
    public enum ContainerType : uint
    {
        Switch = 0,
        Random = 1,
        Random2 = 2,
        Blend = 3,
        Sequence = 4,
#if XLINK_VER_THUNDER || XLINK_VER_EXKING
        Grid,
#endif
#if XLINK_VER_EXKING
        Jump,
#endif
        /* Mono appears to be always last. */
        Mono
    }
}

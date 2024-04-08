using System;

namespace WoomLink.sead;

[Flags]
public enum HeapFlags : ushort
{
    EnableLock              = 1 << 0,
    Disposing               = 1 << 1,
    EnableWarning           = 1 << 2,
    EnableDebugFillSystem   = 1 << 3,
    EnableDebugFillUser     = 1 << 4,
}
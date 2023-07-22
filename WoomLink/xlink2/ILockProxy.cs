using System;

namespace WoomLink.xlink2
{
    public interface ILockProxy : IDisposable
    {
        void Lock();
        void Unlock();
    }
}

using System;
using System.Threading;

namespace WoomLink.xlink2
{
    public class LockProxyForMutex : ILockProxy
    {
        private readonly Mutex Impl = new();

        public void Lock()
        {
            Impl.WaitOne();
        }

        public void Unlock()
        {
            Impl.ReleaseMutex();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Impl.Dispose();
        }
    }
}

using System;
using System.Threading;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct CriticalSection : IDisposable
    {
        private IDisposer Disposer;
        private Mutex Impl;

        public CriticalSection()
        {
            Disposer = new IDisposer();
            Impl = new Mutex();
        }

        public CriticalSection(Pointer<sead.Heap> heap)
        {
            Disposer = new IDisposer(heap, IDisposer.HeapNullOption.UseSpecifiedOrContainHeap);
            Impl = new Mutex();
        }

        public CriticalSection(Pointer<sead.Heap> heap, IDisposer.HeapNullOption option)
        {
            Disposer = new IDisposer(heap, option);
            Impl = new Mutex();
        }

        public void Lock()
        {
            Impl.WaitOne();
        }

        public bool TryLock()
        {
            return Impl.WaitOne(TimeSpan.Zero);
        }

        public void Unlock()
        {
            Impl.ReleaseMutex();
        }

        public void Dispose()
        {
            Impl.Dispose();
            Disposer.Dispose();
        }
    }
}

using System;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct Heap : IDisposable
    {
        public enum HeapDirection
        {
            Reverse = 0,
            Forward = 1,
        }

        private IDisposer Disposer;
        private string Name;
        private UintPointer Start;
        private SizeT Size;
        private Pointer<sead.Heap> Parent;
        private OffsetList<sead.Heap> Children;
        public ListNode ListNode;
        private OffsetList<IDisposer> Disposers;
        private HeapDirection Direction;
        private CriticalSection Mutex;
        private HeapFlags Flags;

        public Heap(
            string name,
            Pointer<Heap> parent,
            UintPointer start,
            SizeT size,
            HeapDirection direction,
            bool threadSafe
        )
        {
            Disposer = new IDisposer(parent, IDisposer.HeapNullOption.UseSpecifiedOrContainHeap);
            Name = name;
            Start = start;
            Size = size;
            Parent = parent;
            ListNode = new ListNode();
            Direction = direction;
            Children = new OffsetList<sead.Heap>(); 
            Disposers = new OffsetList<IDisposer>();
            Mutex = new CriticalSection(parent);
            Flags = HeapFlags.EnableWarning;
            if(threadSafe)
                Flags |= HeapFlags.EnableLock;

            /* TODO: HeapCheckTag? */


            if ((Flags & HeapFlags.EnableLock) != 0)
            {
                try
                {
                    Mutex.Lock();

                    Children.InitOffset();
                    Disposers.InitOffset();
                }
                finally
                {
                    Mutex.Unlock();
                }
            }
            else
            {
                Children.InitOffset();
                Disposers.InitOffset();
            }
        }

        public void AppendDisposer(ref IDisposer disposer)
        {
            try
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Lock();

                Disposers.PushBack(ref disposer);
            }
            finally
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Unlock();
            }
        }

        public void RemoveDisposer(ref IDisposer disposer)
        {
            try
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Lock();

                Disposers.Erase(ref disposer);
            }
            finally
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Unlock();
            }
        }

        public void Destruct()
        {
            try
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Lock();

                Flags |= HeapFlags.Disposing;

                foreach (var disposer in Disposers)
                {
                    disposer.Ref.Dispose();
                }

                Flags &= ~HeapFlags.Disposing;

                /* TODO: removeFromFindContainHeapCache_ */

                if (!Parent.IsNull)
                {
                    Parent.Ref.EraseChild(ref this);
                }
                else
                {
                    /* TODO: removeRootHeap */
                }
            }
            finally
            {
                if ((Flags & HeapFlags.EnableLock) != 0)
                    Mutex.Unlock();
            }

        }

        public void Dispose(UintPointer start, UintPointer end)
        {
            Flags |= HeapFlags.Disposing;

            foreach (var disposer in Disposers)
            {
                if ((start | end) != 0)
                {
                    /* TODO: off by one? */
                    if(disposer.PointerValue < start || end < disposer.PointerValue)
                        continue;
                }
            
                disposer.Ref.Dispose();
            }

            Flags &= ~HeapFlags.Disposing;
        }

        public void EraseChild(ref sead.Heap child)
        {
            try
            {
                Mutex.Lock();
                Children.Erase(ref child);
            }
            finally
            {
                Mutex.Unlock();
            }
        }

        public void Dispose()
        {
            Mutex.Dispose();
            Disposer.Dispose();
        }
    }
}

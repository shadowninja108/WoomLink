using System;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct IDisposer : IDisposable
    {
        //private static readonly Pointer<sead.Heap> DeconstructedHeap = Pointer<sead.Heap>.As(1);

        public enum HeapNullOption
        {
            AlwaysUseSpecifiedHeap = 0,
            UseSpecifiedOrContainHeap = 1,
            DoNotAppendDisposerIfNoHeapSpecified = 2,
            UseSpecifiedOrCurrentHeap = 3,
        }

        public ListNode ListNode;
        private Pointer<sead.Heap> Heap;

        public IDisposer() : this(Pointer<sead.Heap>.Null, HeapNullOption.UseSpecifiedOrContainHeap)
        {
        }

        public IDisposer(Pointer<sead.Heap> heap, HeapNullOption option)
        {
            ListNode = new ListNode();
            var Heap = heap;
            if (!Heap.IsNull)
            {
                Heap.Ref.AppendDisposer(ref this);
                return;
            }

            switch (option)
            {
                case HeapNullOption.AlwaysUseSpecifiedHeap:
                    throw new ArgumentException("Heap must not be null", nameof(Heap), null);
                case HeapNullOption.UseSpecifiedOrContainHeap:
                    if (HeapMgr.Instance == null)
                        return;

                    Heap = HeapMgr.Instance.FindContainHeap(PointerUtil.AsRawPtr(in this));
                    if(!Heap.IsNull)
                        Heap.Ref.AppendDisposer(ref this);
                    break;
                case HeapNullOption.DoNotAppendDisposerIfNoHeapSpecified:
                    break;
                case HeapNullOption.UseSpecifiedOrCurrentHeap:
                    if (HeapMgr.Instance == null)
                        return;

                    Heap = HeapMgr.Instance.GetCurrentHeap();
                    if (!Heap.IsNull)
                        Heap.Ref.AppendDisposer(ref this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        public void Dispose()
        {
            //if (!Heap.Equals(DeconstructedHeap))
            //{
            //    if(!Heap.IsNull)
            //        Heap.Ref.RemoveDisposer(ref this);
            //
            //    Heap = DeconstructedHeap;
            //}
        }
    }
}

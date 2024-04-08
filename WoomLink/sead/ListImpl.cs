using System.Diagnostics;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct ListImpl
    {
        public ListNode StartEnd;
        private int Count;

        public readonly bool IsEmpty => Count == 0;
        public readonly int Size => Count;

        public ListImpl()
        {
            StartEnd.Next = PointerUtil.AsPtr(in StartEnd);
            StartEnd.Prev = PointerUtil.AsPtr(in StartEnd);
        }

        public void PushBack(ref ListNode item)
        {
            StartEnd.InsertFront(ref item);
            Count++;
        }

        public void PushFront(ref ListNode item)
        {
            StartEnd.InsertBack(ref item);
            Count++;
        }

        public void InsertBefore(ref ListNode node, ref ListNode nodeToInsert)
        {
            node.InsertFront(ref nodeToInsert);
            Count++;
        }

        public void InsertAfter(ref ListNode node, ref ListNode nodeToInsert)
        {
            node.InsertBack(ref nodeToInsert);
            Count++;
        }

        public void Erase(ref ListNode node)
        {
            node.Erase();
            Count--;
        }

        public Pointer<ListNode> PopBack()
        {
            if(Count < 1)
                return Pointer<ListNode>.Null;

            var back = StartEnd.Prev;
            back.Ref.Erase();
            Count--;
            return back;
        }

        public Pointer<ListNode> PopFront()
        {
            if(Count < 1)
                return Pointer<ListNode>.Null;

            var front = StartEnd.Next;
            front.Ref.Erase();
            Count--;
            return front;
        }


        public readonly Pointer<ListNode> Front => Count > 0 ? StartEnd.Next : Pointer<ListNode>.Null;
        public readonly Pointer<ListNode> Back => Count > 0 ? StartEnd.Prev : Pointer<ListNode>.Null;

        public readonly Pointer<ListNode> Nth(int n)
        {
            if((uint) Count <= (uint) n)
                return Pointer<ListNode>.Null;

            var node = StartEnd.Next;
            for (var i = 0; i < n; i++)
                node = node.Ref.Next;
            return node;
        }

        public readonly int IndexOf(in ListNode n)
        {
            var node = StartEnd.Next;
            var index = 0;
            while (!node.Equals(PointerUtil.AsPtr(in StartEnd)))
            {
                if (node.Equals(PointerUtil.AsPtr(in n)))
                    return index;
                index++;
                node = node.Ref.Next;
            }

            return -1;
        }

        public void Clear()
        {
            var node = StartEnd.Next;
            while (!node.Equals(PointerUtil.AsPtr(in StartEnd)))
            {
                var next = node.Ref.Next;
                node.Ref.Init();
                node = next;
            }

            Count = 0;
            StartEnd.Prev = PointerUtil.AsPtr(in StartEnd);
            StartEnd.Next = PointerUtil.AsPtr(in StartEnd);
        }

        public void Swap(ref ListNode n1, ref ListNode n2)
        {
            Debug.Assert(!n1.Prev.IsNull && !n1.Next.IsNull && !n2.Prev.IsNull && !n2.Next.IsNull);

            if (PointerUtil.AsRawPtr(in n1) == PointerUtil.AsRawPtr(in n2))
                return;

            var n1Prev = n1.Prev;
            var n2Prev = n2.Prev;

            if (!n2Prev.Equals(PointerUtil.AsRawPtr(in n1)))
            {
                n1.Erase();
                n2Prev.Ref.InsertFront(ref n1);
            }

            if (!n1Prev.Equals(PointerUtil.AsRawPtr(in n2)))
            {
                n2.Erase();
                n1Prev.Ref.InsertFront(ref n2);
            }
        }
    }
}

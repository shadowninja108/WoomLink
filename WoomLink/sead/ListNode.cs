using System.Diagnostics;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct ListNode
    {
        public Pointer<ListNode> Prev;
        public Pointer<ListNode> Next;

        public readonly bool IsLinked => !Prev.IsNull || !Next.IsNull;

        public void Init()
        {
            Prev = Pointer<ListNode>.Null;
            Next = Pointer<ListNode>.Null;
        }

        public void InsertBack(ref ListNode node)
        {
            Debug.Assert(!node.IsLinked);

            var next = Next;
            Next = PointerUtil.AsPtr(in node);
            node.Prev = PointerUtil.AsPtr(in this);
            node.Next = next;

            if (!next.IsNull)
                next.Ref.Prev = PointerUtil.AsPtr(in node);
        }

        public void InsertFront(ref ListNode node)
        {
            Debug.Assert(!node.IsLinked);

            var prev = Prev;
            Prev = PointerUtil.AsPtr(in node);
            node.Prev = prev;
            node.Next = PointerUtil.AsPtr(in this);
            if (prev.IsNull)
                return;

            prev.Ref.Next = PointerUtil.AsPtr(in node);
        }

        public void Erase()
        {
            if (!Prev.IsNull)
                Prev.Ref.Next = Next;
            if(!Next.IsNull)
                Next.Ref.Prev = Prev;

            Prev = Pointer<ListNode>.Null;
            Next = Pointer<ListNode>.Null;
        }
    }
}

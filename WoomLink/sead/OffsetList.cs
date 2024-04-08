using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct OffsetList<T> : IEnumerable<Pointer<T>> where T : struct
    {
        private ListImpl Impl;
        private int Offset;

        public void InitOffset(int offset)
        {
            Offset = offset;
        }

        public void InitOffset(in T obj, in ListNode node)
        {
            InitOffset((int)PointerUtil.Subtract(in node, in obj));
        }

        public void InitOffset()
        {
            InitOffset((int)Marshal.OffsetOf<T>("ListNode"));
        }

        public void PushBack(ref T item)
        {
            Impl.PushBack(ref ObjToListNode(in item).Ref);
        }

        public void PushFront(ref T item)
        {
            Impl.PushFront(ref ObjToListNode(in item).Ref);
        }

        public Pointer<T> PopBack()
        {
            return Impl.PopBack().Cast<T>();
        }

        public Pointer<TListNode<T>> PushBack()
        {
            return Impl.PopFront().Cast<TListNode<T>>();
        }

        public void InsertBefore(in T node, T nodeToInsert)
        {
            Impl.InsertBefore(ref ObjToListNode(in node).Ref, ref ObjToListNode(in nodeToInsert).Ref);
        }

        public void InsertAfter(in T node, T nodeToInsert)
        {
            Impl.InsertAfter(ref ObjToListNode(in node).Ref, ref ObjToListNode(in nodeToInsert).Ref);
        }

        public void Erase(ref T item) => Impl.Erase(ref ObjToListNode(in item).Ref);
        public readonly Pointer<T> Front => Impl.Front.Cast<T>();
        public readonly Pointer<T> Back => Impl.Back.Cast<T>();
        public readonly Pointer<T> Nth(int n) => Impl.Nth(n).Cast<T>();
        public readonly int IndexOf(in T node) => Impl.IndexOf(ObjToListNode(in node).Ref);
        public void Swap(ref T n1, ref T n2) => Impl.Swap(ref ObjToListNode(in n1).Ref, ref ObjToListNode(in n2).Ref);
        public void Clear() => Impl.Clear();

        public readonly Pointer<T> Prev(in T node)
        {
            var prevNode = ObjToListNode(in node).Ref.Prev.Cast<T>();
            if (prevNode.Equals(PointerUtil.AsRawPtr(in Impl.StartEnd)))
                return Pointer<T>.Null;

            return prevNode;
        }

        public readonly Pointer<T> Next(in T node)
        {
            var nextNode = ObjToListNode(in node).Ref.Next.Cast<T>();
            if (nextNode.Equals(PointerUtil.AsRawPtr(in Impl.StartEnd)))
                return Pointer<T>.Null;

            return nextNode;
        }

        private readonly Pointer<ListNode> ObjToListNode(in T obj)
        {
            return PointerUtil.AsPtr(in obj).Cast<ListNode>().Add(Offset);
        }

        private readonly Pointer<T> ListNodeToObj(in ListNode node)
        {
            return PointerUtil.AsPtr(in node).Cast<T>().SubBytes((UintPointer) Offset);
        }

        private readonly Pointer<T> ListNodeToObjWithNullCheck(Pointer<ListNode> node)
        {
            return !node.IsNull ? ListNodeToObj(in node.Ref) : Pointer<T>.Null;
        }

        public IEnumerator<Pointer<T>> GetEnumerator()
        {
            var next = Front;
            while (!next.IsNull)
            {
                yield return next;
                var nextNode = ObjToListNode(in next.Ref);
                next = ListNodeToObj(in nextNode.Ref.Next.Ref);
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

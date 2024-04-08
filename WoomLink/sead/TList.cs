using System.Collections;
using System.Collections.Generic;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct TList<T> : IEnumerable<T> where T : struct
    {
        public ListImpl Impl;


        public void PushBack(ref TListNode<T> item)
        {
            item.Erase();
            item.List = PointerUtil.AsPtr(in this);
            Impl.PushBack(ref item.Node);
        }

        public void PushFront(ref TListNode<T> item)
        {
            item.Erase();
            item.List = PointerUtil.AsPtr(in this);
            Impl.PushFront(ref item.Node);
        }

        public Pointer<TListNode<T>> PopBack()
        {
            return Impl.PopBack().Cast<TListNode<T>>();
        }

        public Pointer<TListNode<T>> PushBack()
        {
            return Impl.PopFront().Cast<TListNode<T>>();
        }

        public void InsertBefore(ref TListNode<T> node, TListNode<T> nodeToInsert)
        {
            Impl.InsertBefore(ref node.Node, ref nodeToInsert.Node);
        }

        public void InsertAfter(ref TListNode<T> node, TListNode<T> nodeToInsert)
        {
            Impl.InsertAfter(ref node.Node, ref nodeToInsert.Node);
        }

        public void Erase(ref TListNode<T> item)
        {
            if (item.List.IsNull)
                return;

            item.List = Pointer<TList<T>>.Null;
            Impl.Erase(ref item.Node);
        }

        public readonly Pointer<TListNode<T>> Front => Impl.Front.Cast<TListNode<T>>();
        public readonly Pointer<TListNode<T>> Back => Impl.Back.Cast<TListNode<T>>();
        public readonly Pointer<TListNode<T>> Nth(int n) => Impl.Nth(n).Cast<TListNode<T>>();
        public readonly int IndexOf(in TListNode<T> node) => Impl.IndexOf(node.Node);
        public void Swap(ref TListNode<T> n1, ref TListNode<T> n2) => Impl.Swap(ref n1.Node, ref n2.Node);
        public void Clear() => Impl.Clear();

        public readonly Pointer<TListNode<T>> Prev(in TListNode<T> node)
        {
            var prevNode = node.Node.Prev.Cast<TListNode<T>>();
            if(prevNode.Equals(PointerUtil.AsRawPtr(in Impl.StartEnd)))
                return Pointer<TListNode<T>>.Null;

            return prevNode;
        }

        public readonly Pointer<TListNode<T>> Next(in TListNode<T> node)
        {
            var nextNode = node.Node.Next.Cast<TListNode<T>>();
            if(nextNode.Equals(PointerUtil.AsRawPtr(in Impl.StartEnd)))
                return Pointer<TListNode<T>>.Null;
            
            return nextNode;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var next = Front;
            while (!next.IsNull)
            {
                yield return next.Value.Data;
                next = next.Ref.Node.Next.Cast<TListNode<T>>();
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

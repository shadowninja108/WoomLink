using WoomLink.Ex;

namespace WoomLink.sead
{
    public struct TListNode<T> where T : struct
    {
        public ListNode Node;
        public T Data;
        public Pointer<TList<T>> List;

        public void Erase()
        {
            var list = List;
            if (!list.IsNull)
                list.Ref.Erase(ref this);
        }
    }
}

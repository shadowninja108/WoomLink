namespace WoomLink.sead
{
    /*public class PtrArray<T> : IList<T>
    {
        private TList<T>? _list;

        public bool IsFull => _list!.Count == _list.Capacity;
        public int Count => _list!.Count;

        public bool IsReadOnly => true;


        public PtrArray()
        {
            _list = null;
        }

        public PtrArray(int capacity)
        {
            _list = new TList<T>(capacity);
        }

        public void AllocBuffer(int size /* heap #1# /* alignment #1#)
        {
            _list = new TList<T>(size);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list!.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list!).GetEnumerator();
        }

        public void Add(T item)
        {
            if (IsFull)
            {
                throw new InvalidOperationException("List is full.");
            }
            _list!.Add(item);
        }

        public void Clear()
        {
            _list!.Clear();
        }

        public bool Contains(T item)
        {
            return _list!.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var maxCopyLength = _list!.Capacity - arrayIndex;
            if (maxCopyLength > array.Length)
                throw new InvalidOperationException("Will not fit in list.");

            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _list!.Remove(item);
        }

        public int IndexOf(T item)
        {
            return _list!.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list!.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list!.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _list![index];
            set => _list![index] = value;
        }
    }*/
}

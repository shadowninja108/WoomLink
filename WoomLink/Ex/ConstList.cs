using System;
using System.Collections;
using System.Collections.Generic;

namespace WoomLink.Ex
{
    public class ConstList<T> : IList<T>, IDisposable
        where T : struct
    {
        private ResizableList<T> Source;
        public int Count => Source.Count;
        public bool IsReadOnly => true;

        public ConstList(ResizableList<T> source)
        {
            if (!source.Mutable)
            {
                throw new InvalidOperationException("Can't have multiple const accesses to mutable list!");
            }
            Source = source;
            Source.Mutable = false;
        }

        public T this[int index]
        {
            get => Source[index];
            set => Source[index] = value;
        }
        public bool Contains(T item) => Source.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => Source.CopyTo(array, arrayIndex);
        public int IndexOf(T item) => Source.IndexOf(item);
        public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Source.GetEnumerator();
        public ref T At(int index) => ref Source.ElementData[index];
        public Span<T> AsSpan() => Source.ElementData;
        public static implicit operator Span<T>(ConstList<T> list) => list.AsSpan();

        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public void Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        public void Dispose()
        {
            Source.Mutable = true;
        }
    }
}

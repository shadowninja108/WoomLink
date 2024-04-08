using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WoomLink.Ex
{
    public class ResizableList<T>(int initialCapacity) : IList<T> where T : struct
    {
        private const int DefaultCapacity = 10;

        internal T[] ElementData = new T[initialCapacity];
        private int Size;

        public int Count => Size;
        public bool IsReadOnly => true;

        /* TODO: make thread safe. */
        internal bool Mutable = false;

        public ResizableList() : this(DefaultCapacity) { }

        public T this[int index]
        {
            get => ElementData[index];
            set => ElementData[index] = value;
        }

        public void Add(T item)
        {
            EnsureMutable();
            EnsureCapacity(ElementData.Length+1);
            ElementData[^1] = item;
        }

        public void Clear()
        {
            EnsureMutable();
            ElementData = Array.Empty<T>();
            Size = 0;
        }

        public bool Contains(T item) => ElementData.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            ElementData.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, T item)
        {
            EnsureMutable();
            EnsureCapacity(Size+1);
            Copy(ElementData, index, ElementData, index + 1, Size - index);
            Size++;
        }

        public void RemoveAt(int index)
        {
            EnsureMutable();
            var numMoved = Size - index - 1;
            if (numMoved > 0)
            {
                Copy(ElementData, index + 1, ElementData, index, numMoved);
            }
            Size--;
        }

        public bool Remove(T item)
        {
            EnsureMutable();
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public ConstList<T> Const() => new(this);

        public void TrimToSize()
        {
            var oldCapacity = ElementData.Length;
            if (Size < oldCapacity)
            {
                ElementData = CopyOf(ElementData, Size);
            }
        }

        private void EnsureMutable()
        {
            if (!Mutable)
            {
                throw new InvalidOperationException("Modified list while in immutable state!");
            }
        }

        private void EnsureCapacity(int minCapacity)
        {
            if(minCapacity - ElementData.Length > 0)
                Grow(minCapacity);
        }

        private void Grow(int minCapacity)
        {
            var oldCapacity = ElementData.Length;
            var newCapacity = oldCapacity + (oldCapacity >> 1);
            if (newCapacity - minCapacity < 0)
                newCapacity = minCapacity;
            ElementData = CopyOf(ElementData, newCapacity);
        }

        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)ElementData.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static void Copy(ReadOnlySpan<T> sourceArray, int sourceIndex, Span<T> destinationArray, int destinationIndex, int length)
        {
            var source = sourceArray[sourceIndex..(sourceIndex + length)];
            var destination = destinationArray[destinationIndex..(destinationIndex + length)];
            source.CopyTo(destination);
        }

        private static T[] CopyOf(T[] array, int size)
        {
            var n = new T[size];
            array[..Math.Min(array.Length, size)].CopyTo(n, 0);
            return n;
        }
    }
}

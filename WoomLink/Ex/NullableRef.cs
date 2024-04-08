using System;

namespace WoomLink.Ex
{
    public ref struct NullableRef<T> where T : new()
    {
        /* Is this really the best solution? */
        private static T Dummy = new();

        private ref T _ref;

        public bool IsNull { get; }

        public readonly ref T Ref
        {
            get
            {
                if (IsNull) 
                    throw new Exception("Accessed null ref!");
                
                return ref Ref;
            }
        }

        public NullableRef()
        {
            IsNull = true;
            _ref = ref Dummy;
        }

        public NullableRef(ref T value)
        {
            IsNull = false;
            _ref = ref value;
        }

        public static NullableRef<T> Null => new();
    }
}

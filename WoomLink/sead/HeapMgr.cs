using System;
using WoomLink.Ex;

namespace WoomLink.sead
{
    public class HeapMgr
    {
        public static HeapMgr? Instance = new();
        private static readonly Arena DefaultArena = new();

        public static Arena? Arena;
        private static TimeSpan SleepSpanAtRemoveCacheFailure = TimeSpan.Zero;
        

        private Pointer<sead.Heap> DefaultHeap;
        private Func<int>? AllocFailedCallback;

        public static void Initialize(SizeT size)
        {
            /* TODO: lock */
            Arena = DefaultArena;
            DefaultArena.Initialize(size);
            Instance.InitializeImpl();
        }

        private void InitializeImpl()
        {
            AllocFailedCallback = null;
            /* TODO: */
            SleepSpanAtRemoveCacheFailure = TimeSpan.FromMilliseconds(1);
            //Arena!.
        }

        public Pointer<sead.Heap> GetCurrentHeap()
        {
            return Pointer<sead.Heap>.Null;
        }

        public Pointer<sead.Heap> FindContainHeap(UintPointer pointer)
        {
            return Pointer<sead.Heap>.Null;
        }
    }
}

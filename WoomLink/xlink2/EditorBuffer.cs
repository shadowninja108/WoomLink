using WoomLink.Ex;
using WoomLink.xlink2.File;

namespace WoomLink.xlink2
{
    public class EditorBuffer
    {
        public System System;
        /* Heap */
        public ResizableList<EditorResourceParam> Params;
        public byte[] ReceiveBuffer;
        public string Field40 = "";
        public Pointer<ParamDefineTable> PDT;
        public UintPointer RawPDT;

        public EditorBuffer(System system /* heap */)
        {
            System = system;
            PDT = FakeHeap.AllocateT<ParamDefineTable>(1);
            //RawPDT = Heap2.Allocate(0x800);
        }

        public void Destroy()
        {
            /* Free raw bin... */
            /* Free param list... */
            /* Free raw pdt... */
            /* Destroy pdt... */
        }

        public byte[] AllocReceiveBuffer(uint size)
        {
            /* Destroy existing ReceiveBuffer if needed. */
            /* Ensure we can allocate enough for the buffer on the heap. */

            ReceiveBuffer = new byte[size];
            return ReceiveBuffer;
        }

        public void ReadFinished()
        {
            /* TODO */
        }

        public void SetupParamDefineTable(UintPointer pointer, uint size)
        {
            var source = Pointer<byte>.As(pointer);
            var dest = Pointer<byte>.As(RawPDT);

            /* Unchecked size copy. */
            source.AsSpan((int)size).CopyTo(dest.AsSpan((int)size));

            ref var pdt = ref PDT.Ref;
            if (pdt.Initialized)
            {
                pdt.Reset();
            }

            /* TODO: DebugOperationParam */
            pdt.Setup(RawPDT, System.GetUserParamNum(), false);
        }

        public void ApplyGlobalPropertyDefinition()
        {
            using var paramss = Params.Const();
            foreach (ref var param in paramss.AsSpan())
            {
                ResourceParamCreator.SolveAboutGlobalProperty(ref param, in PDT.Ref, System);
            }
        }
    }

}

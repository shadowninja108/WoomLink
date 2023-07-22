using WoomLink.sead;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public class ResourceBuffer
    {
        private const uint Magic = 0x4B4E4C58; /* XLNK */

        public static readonly RomResourceParam EmptyRomResourceParam = new() { Setup = true };
        public static readonly Pointer<ResUserHeader> EmptyUserHeader;

        static ResourceBuffer()
        {
            EmptyUserHeader = Heap.AllocateT<ResUserHeader>(1);
            EmptyUserHeader.Ref = new ResUserHeader() { IsSetup = 1 };
        }

        public ParamDefineTable PDT;
        public RomResourceParam RSP;

        public bool Load(UintPointer start, System system)
        {
            var headerPtr = Pointer<ResourceHeader>.As(start);
            //Utils.MaybeAdjustEndianness(typeof(ResourceHeader), ref headerPtr.Ref, Endianness.Big);
            ref var header = ref headerPtr.Ref;

            /* Ensure magic matches. */
            if (header.Magic != Magic)
                return false;

            /* Ensure version matches. */
            if (header.Version != system.GetResourceVersion())
                return false;

            /* Don't continue if we're already setup. */
            if (RSP.Setup || PDT.Initialized)
                return false;

            /* Skip user offset/hashes.  */
            var pdtStart =
                headerPtr.Add(1)
                .Cast<uint>().Add(header.NumUser)
                .Cast<UintPointer>().Add(header.NumUser)
                .AlignUp(Heap.PointerSize).PointerValue;

            /* TODO: DebugOperationParam */
            PDT.Setup(pdtStart, system.GetUserParamNum(), false);

            ResourceParamCreator.CreateParamAndSolveResource(ref RSP, headerPtr, in PDT, system);

            Locator.UpdateDataLoadedCount();

            return true;
        }

        public Pointer<ResUserHeader> SearchResUserHeader(string name)
        {
            if (RSP.NumUser == 0)
                return Pointer<ResUserHeader>.Null;
            

            var hash = HashCrc32.CalcStringHash(name);
            var search = Utils.BinarySearch<uint, uint>(RSP.UserDataHashes.AsSpan(RSP.NumUser), hash);
            if (search < 0)
                return Pointer<ResUserHeader>.Null;

            return RSP.UserDataPointersSpan[search];
        }

        public void ApplyGlobalPropertyDefinition(System system)
        {
            if(RSP.Setup)
                ResourceParamCreator.SolveAboutGlobalProperty(ref RSP, in PDT, system);
        }
    }
}

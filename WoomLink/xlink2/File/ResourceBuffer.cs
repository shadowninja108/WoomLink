using WoomLink.Ex;
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
            EmptyUserHeader = Ex.FakeHeap.AllocateT<ResUserHeader>(1);
            EmptyUserHeader.Ref = new ResUserHeader() { IsSetup = 1 };
        }

        public ParamDefineTable PDT;
        public RomResourceParam RSP;

        public ResourceBuffer()
        {
            PDT.Reset();
        }

        public bool Load(UintPointer data, System system)
        {
            var headerPtr = Pointer<ResourceHeader>.As(data);
            ref var header = ref headerPtr.Ref;

            /* Ensure magic matches. */
            if (header.Magic != Magic)
                return false;

            /* Ensure version matches. */
            var supportedVersion = system.GetResourceVersion();
            if (header.Version != supportedVersion)
            {
                system.AddError(Error.Type.DataVersionError, null, "Program:{0} Resource:{1}", supportedVersion, header.Version);
                return false;
            }

            /* Don't continue if we're already setup. */
            if (RSP.Setup || PDT.Initialized)
                return false;

            /* Skip user offset/hashes.  */
            var pdtStart =
                headerPtr.Add(1)
                .Cast<uint>().Add(header.NumUser)
                .Cast<UintPointer>().Add(header.NumUser)
                .AlignUp(FakeHeap.PointerSize).PointerValue;

            /* TODO: DebugOperationParam */
            PDT.Setup(pdtStart, system.GetUserParamNum(), false);

            ResourceParamCreator.CreateParamAndSolveResource(ref RSP, data, in PDT, system);

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

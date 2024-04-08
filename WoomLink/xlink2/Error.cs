namespace WoomLink.xlink2
{
    public struct Error
    {
        public enum Type
        {
            None = 0x0,
            DelayDisturbed = 0x1,
            OtameshiEmitFailed = 0x2,
            AssetPreviewFailed = 0x3,
            EventPoolFull = 0x4,
            OneTimeShoudBeClipNoneOrClipKill = 0x5,
            HoldAssetFull = 0x6,
            NotFoundEmitterSet = 0x7,
            NotFoundAsset = 0x8,
            NotFoundBone = 0x9,
            NotFoundGroup = 0xA,
            NotFoundDRCS = 0xB,
            MtxSetTypeIsNotProgrammer = 0xC,
            NotInputRootMtx = 0xD,
            DataFormatError = 0xE,
            DataVersionError = 0xF,
            ResourceAccessFailed = 0x10,
            NotFoundAction = 0x11,
            NotFoundActionSlot = 0x12,
            NotFoundProperty = 0x13,
            InvalidContainer = 0x14,
            CommProtocolVersionError = 0x15,
            PropertyDefineError = 0x16,
            ParameterRangeError = 0x17,
            CustomParamAccessFailed = 0x18,
            CallWithoutSetup = 0x19,
            SoundLimitTargetOver = 0x1A,
            EventHasMultipleAsset = 0x1B,
            ListToGetterIsFull = 0x1C,
            PropertyOutOfRange = 0x1D,
            InvalidPropertyType = 0x1E,
            OutOfMemory = 0x1F,
            InterruptCalc = 0x20,
            TooMuchBlendChild = 0x21,
            MultipleDefinedKeyName = 0x22,
            EmitInvalidEmitterSet = 0x23,
            Etc = 0x24,
            Unknown = 0x25,
        }

        public Type ErrorType;
        /* TODO: FixedSafeString? */
        public string UserName;
        public string Message;
        public int Field278;
    }
}

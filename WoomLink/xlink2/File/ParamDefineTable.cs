using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public struct ParamDefineTable
    {
        public int NumAllParams;
        public int NumTotalAssetParams;
        public int NumTriggerParams;
        public Pointer<ParamDefine> UserParam;
        public Pointer<ParamDefine> AssetParam;
        public Pointer<ParamDefine> TriggerParam;
        private Pointer<char> StringTable;
        public int NumUserAssetParams;
        public int NumStandardAssetParams;
        public int NumNonUserParams;
        public int NumUserParams;
        public int TotalSize;
        public bool Initialized;


        public Span<ParamDefine> UserParamSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => UserParam.AsSpan(NumAllParams);
        }

        public Span<ParamDefine> AssetParamSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AssetParam.AsSpan(NumTotalAssetParams);
        }

        public Span<ParamDefine> TriggerParamSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TriggerParam.AsSpan(NumTriggerParams);
        }

        public void Setup(UintPointer data, uint userParamNum, bool showDebug)
        {
            if (Initialized) 
                return;

            var headerPtr = Pointer<ParamDefineTableHeader>.As(data);
            //Utils.MaybeAdjustEndianness(typeof(ResourceHeader), ref headerPtr.Ref, Endianness.Big);
            ref var header = ref headerPtr.Ref;

            TotalSize = header.Size;
            var totalUserParams = header.NumTotalUserParams;
            NumAllParams = totalUserParams;
            NumNonUserParams = (int)(totalUserParams - userParamNum);
            NumUserParams = (int)userParamNum;
            var totalAssetParams = header.NumTotalAssetParams;
            NumTotalAssetParams = totalAssetParams;
            var numUserAssetParams = header.NumUserAssetParams;
            NumUserAssetParams = numUserAssetParams;
            NumStandardAssetParams = totalAssetParams - numUserAssetParams;
            NumTriggerParams = header.NumTriggerParams;
            var startOfParams = headerPtr.AtEnd<ParamDefine>().AlignUp(Heap.PointerSize);
            UserParam = startOfParams;
            AssetParam = UserParam.Add(header.NumTotalUserParams);
            TriggerParam = AssetParam.Add(header.NumTotalAssetParams);
            StringTable = TriggerParam.Add(header.NumTriggerParams).Cast<char>();

            //Utils.MaybeAdjustEndianness(typeof(ResourceHeader), UserParamSpan, Endianness.Big);
            //Utils.MaybeAdjustEndianness(typeof(ResourceHeader), AssetParamSpan, Endianness.Big);
            //Utils.MaybeAdjustEndianness(typeof(ResourceHeader), TriggerParamSpan, Endianness.Big);

            Debug.Assert(StringTable.PointerValue < data + (ulong)header.Size);

            static void Solve(ref ParamDefine define, UintPointer stringTable)
            {
                define.Name.PointerValue += stringTable;

                if (define.Type == ParamType.String)
                    define.DefaultValueAsString.PointerValue += stringTable;
            }

            foreach (ref var define in UserParamSpan)
            {
                Solve(ref define, StringTable.PointerValue);
            }

            foreach (ref var define in AssetParamSpan)
            {
                Solve(ref define, StringTable.PointerValue);
            }

            foreach (ref var define in TriggerParamSpan)
            {
                Solve(ref define, StringTable.PointerValue);
            }

            Initialized = true;
        }

        public string GetAssetParamDefaultValueString(uint reff)
        {
            if (NumTotalAssetParams <= reff)
                return "";
            return AssetParamSpan[(int)reff].DefaultValueAsString.AsString();
        }

        public int GetAssetParamDefaultValueInt(uint reff)
        {
            if (NumTotalAssetParams <= reff)
                return 0;
            return AssetParamSpan[(int)reff].DefaultValueAsInt;
        }

        public float GetAssetParamDefaultValueFloat(uint reff)
        {
            if (NumTotalAssetParams <= reff)
                return 0;
            return AssetParamSpan[(int)reff].DefaultValueAsFloat;
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2
{
    public struct CommonResourceParam
    {
        public int NumResParam;
        public int NumResAssetParam;
        public int NumResTriggerOverwriteParam;
        public int NumLocalPropertyNameRefTable;
        public int NumLocalPropertyEnumNameRefTable;
        public int NumDirectValueTable;
        public int NumRandomTable;
        public int NumCurveTable;
        public int NumCurvePointTable;
        public Pointer<ResAssetParam> ResAssetParamTable;
        public Pointer<ResTriggerOverwriteParam> TriggerOverwriteParamTable;
        public Pointer<Pointer<char>> LocalPropertyNameRefTable;
        public Pointer<Pointer<char>> LocalPropertyEnumNameRefTable;
        public Pointer<uint> DirectValueTable;
        public Pointer<ResRandomCallTable> RandomTable;
        public Pointer<ResCurveCallTable> CurveTable;
        public Pointer<CurvePointTable> CurvePointTable;
        public Pointer<ResCondition> ConditionTable;
        public Pointer<ResUserHeader> ExRegionPointer;
        public UintPointer NameTablePointer;

        public Span<ResAssetParam> ResAssetParamTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ResAssetParamTable.AsSpan(NumResAssetParam);
        }

        public Span<ResTriggerOverwriteParam> TriggerOverwriteParamTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TriggerOverwriteParamTable.AsSpan(NumResTriggerOverwriteParam);
        }
        public Span<Pointer<char>> LocalPropertyNameRefTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LocalPropertyNameRefTable.AsSpan(NumLocalPropertyNameRefTable);
        }
        public Span<Pointer<char>> LocalPropertyEnumNameRefTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LocalPropertyEnumNameRefTable.AsSpan(NumLocalPropertyEnumNameRefTable);
        }
        public Span<uint> DirectValueTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DirectValueTable.AsSpan(NumDirectValueTable);
        }
        public Span<int> DirectValueTableSpanAsInts
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DirectValueTable.Cast<int>().AsSpan(NumDirectValueTable);
        }
        public Span<float> DirectValueTableSpanAsFloats
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DirectValueTable.Cast<float>().AsSpan(NumDirectValueTable);
        }
        public Span<ResRandomCallTable> RandomTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RandomTable.AsSpan(NumRandomTable);
        }
        public Span<ResCurveCallTable> CurveTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CurveTable.AsSpan(NumCurveTable);
        }
        public Span<CurvePointTable> CurvePointTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CurvePointTable.AsSpan(NumCurvePointTable);
        }
    }
}

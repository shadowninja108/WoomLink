using System;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.File.Structs;

namespace WoomLink.xlink2.File
{
    public struct UserBinParam
    {
        public Pointer<ResUserHeader> ResUserHeader;
        public Pointer<LocalPropertyRef> LocalPropertyRefArry;
        public Pointer<ResParam> UserParamArry;
        public Pointer<ushort> SortedAssetIdTable;
        public Pointer<ResAssetCallTable> AssetCallTable;
        public UintPointer ContainerTablePos;
        public Pointer<ResActionSlot> ResActionSlotTable;
        public Pointer<ResAction> ActionTable;
        public Pointer<ResActionTrigger> ActionTriggerTable;
        public Pointer<ResProperty> PropertyTable;
        public Pointer<ResPropertyTrigger> ResPropertyTriggerTable;
        public Pointer<ResAlwaysTrigger> ResAlwaysTriggerTable;

        public ref ResUserHeader Header
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref ResUserHeader.Ref;
        }
        public Span<LocalPropertyRef> LocalPropertyRefArrySpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LocalPropertyRefArry.AsSpan(Header.NumLocalProperty);
        }
        public Span<ushort> SortedAssetIdTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => SortedAssetIdTable.AsSpan(Header.NumCallTable);
        }
        public Span<ResAssetCallTable> AssetCallTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AssetCallTable.AsSpan(Header.NumCallTable);
        }
        public Span<ResActionSlot> ResActionSlotTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ResActionSlotTable.AsSpan(Header.NumResActionSlot);
        }
        public Span<ResAction> ActionTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ActionTable.AsSpan(Header.NumResAction);
        }
        public Span<ResActionTrigger> ActionTriggerTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ActionTriggerTable.AsSpan(Header.NumResActionTrigger);
        }
        public Span<ResProperty> PropertyTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PropertyTable.AsSpan(Header.NumResProperty);
        }
        public Span<ResPropertyTrigger> ResPropertyTriggerTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ResPropertyTriggerTable.AsSpan(Header.NumResPropertyTrigger);
        }
        public Span<ResAlwaysTrigger> ResAlwaysTriggerTableSpan
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ResAlwaysTriggerTable.AsSpan(Header.NumResAlwaysTrigger);
        }
    }
}

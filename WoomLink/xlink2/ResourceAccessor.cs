using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using WoomLink.Ex;
using WoomLink.sead;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.User.Instance;
using WoomLink.xlink2.User.Resource;

namespace WoomLink.xlink2
{
    public abstract class ResourceAccessor
    {
        public Pointer<ResUserHeader> HeaderPointer;
        public UserResource? Resource;
        public System System;

        protected ResourceAccessor(UserResource resource, System system)
        {
            Resource = resource;
            System = system;
        }

        public bool CheckAndErrorIsAsset(in ResAssetCallTable table, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] args)
        {
            if (!table.IsContainer)
                return true;

            var message = string.Format(format, args);
            SetError("{0}: [{1}] is container", message, table.KeyName.AsString());
            return false;
        }

        public Pointer<ResParam> GetResParamFromAssetParamPos(Pointer<ResAssetParam> param, uint reff)
        {
            ref var resParam = ref param.Ref;
            if (!resParam.IsParamDefault(reff))
                return param.GetValues().Add((UintPointer)(BitFlagUtil.CountRightOnBit64(resParam.Bitfield, (int)reff) - 1));
            else
                return Pointer<ResParam>.Null;
        }

        protected Pointer<char> GetResParamValueString(in ResParam param)
        {
            return param.GetAsString(in Resource!.CurrentParam!.Common);
        }

        protected int GetResParamValueInt(in ResParam param)
        {
            return param.GetAsInt(in Resource!.CurrentParam!.Common);
        }

        protected float GetResParamValueFloat(in ResParam param, UserInstance instance)
        {
            ref var commonParam = ref Resource!.CurrentParam!.Common;
            switch (param.ReferenceType)
            {
                case ValueReferenceType.Direct:
                    return GetResParamValueInt(in param);

                case ValueReferenceType.Curve:
                    return GetCurveValue(in param.GetAsCurve(in commonParam), instance);
                case ValueReferenceType.Unk3:
                {
                    if (commonParam.RandomTable.IsNull)
                        return 0;

                    ref var random = ref param.GetAsRandom(in commonParam);
                    var range = random.MaxValue - random.MinValue;

                    return random.MinValue + range * System.Rnd.GetSingle();
                }
                case ValueReferenceType.Unk6:
                {
                    if (commonParam.RandomTable.IsNull)
                        return 0;

                    ref var random = ref param.GetAsRandom(in commonParam);
                        var range = random.MaxValue - random.MinValue;
                    if (range <= 0)
                        range = -range;
                    range *= 0.5f;

                    var rnd = System.Rnd.GetSingle();
                    rnd *= rnd;
                    var neg = rnd < 0;
                    if (rnd <= 0)
                        rnd = -rnd;

                    float b;
                    if (neg)
                        b = -(range * (rnd * rnd));
                    else
                        b = range * (rnd * rnd);

                    return random.MinValue + range + b;
                }
                case ValueReferenceType.Unk7:
                    throw new NotImplementedException();
                case ValueReferenceType.Unk8:
                    throw new NotImplementedException();
                case ValueReferenceType.Unk9:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkA:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkB:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkC:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkD:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkE:
                    throw new NotImplementedException();
                case ValueReferenceType.UnkF:
                    throw new NotImplementedException();
                case ValueReferenceType.Unk10:
                    throw new NotImplementedException();
                case ValueReferenceType.Unk11:
                    throw new NotImplementedException();
                default:
                    SetError("GetFloat: invalidReferenceType {0}", (int) param.ReferenceType);
                    return 0;
            }
        }

        public float GetCurveValue(in ResCurveCallTable table, UserInstance instance)
        {
            throw new NotImplementedException();
        }

        protected Pointer<char> GetResOverwriteParamValueString(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff)
        {
            var id = GetTriggerOverwriteParamId(reff);

            if (paramPtr.IsNull)
                return FakeHeap.EmptyString;

            if (id < 0)
                return FakeHeap.EmptyString;

            ref var param = ref paramPtr.Ref;
            if (param.IsParamDefault(reff))
                return FakeHeap.EmptyString;

            var index = BitFlagUtil.CountRightOnBit(param.Bitfield, (int)reff);
            return GetResParamValueString(in paramPtr.GetValuesSpan()[index]);
        }

        protected float GetResOverwriteParamValueFloat(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff, UserInstance instance)
        {
            var id = GetTriggerOverwriteParamId(reff);
            if (paramPtr.IsNull)
                return 0;
            if (id < 0)
                return 0;
            
            ref var param = ref paramPtr.Ref;
            if(param.IsParamDefault(reff)) 
                return 0;

            ref var define = ref paramPtr.GetValuesSpan()[BitFlagUtil.CountRightOnBit(param.Bitfield, (int)reff)];
            return GetResParamValueFloat(in define, instance);
        }

        public bool IsParamOverwritten(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff)
        {
            var id = GetTriggerOverwriteParamId(reff);

            if (paramPtr.IsNull)
                return false;
            
            if(id < 0)
                return false;

            return !paramPtr.Ref.IsParamDefault(reff);
        }

        /* These are probably a macro? */
        protected int GetIntParamFromAsset(in ResAssetCallTable table, string funcName, uint index)
        {
            if (!CheckAndErrorIsAsset(in table, funcName))
                return 0;

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, index);
            int result;
            if (!param.IsNull)
                result = GetResParamValueInt(in param.Ref);
            else
                result = System.GetParamDefineTable().GetAssetParamDefaultValueInt(index);

            return result;
        }

        protected float GetFloatParamFromAsset(in ResAssetCallTable table, string funcName, uint index, UserInstance instance) 
        {
            if (!CheckAndErrorIsAsset(in table, funcName))
                return 0;

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, index);
            float result;
            if (!param.IsNull)
                result = GetResParamValueFloat(in param.Ref, instance);
            else
                result = System.GetParamDefineTable().GetAssetParamDefaultValueFloat(index);

            return result;
        }

        protected Pointer<char> GetStringParamFromAsset(in ResAssetCallTable table, string funcName, uint index)
        {
            if (!CheckAndErrorIsAsset(in table, funcName))
                return FakeHeap.EmptyString;

            var param = GetResParamFromAssetParamPos(table.ParamAsAsset, index);
            Pointer<char> result;
            if (!param.IsNull)
                result = GetResParamValueString(in param.Ref);
            else
                result = System.GetParamDefineTable().GetAssetParamDefaultValueString(index);

            return result;
        }

        public bool IsNeedObserve(in ResAssetCallTable table)
        {
            var resource = Resource;
            if(resource == null)
                return false;

            var param = resource.CurrentParam!;



            /* They normally do pointer arithmetic here assuming the pointer is in this array. I don't think there's an equivalent here, so I'll search by asset ID instead. */
            //var index = -1;
            //var acts = param.User.AssetCallTableSpan;
            //for (var i = 0; i < acts.Length; i++)
            //{
            //    if (acts[i].AssetId != table.AssetId) 
            //        continue;
            //    
            //    index = i;
            //    break;
            //}

            /* TODO: check if this actually works? */
            var index = PointerUtil.Subtract(in table, in param.User.AssetCallTable.Ref) / Unsafe.SizeOf<ResAssetCallTable>();
            return param.CallTables[index].IsNeedObserve;
        }

        private void SetError([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] args)
        {
            var user = Resource?.User;
            var message = string.Format(format, args);
            System.AddError(Error.Type.ResourceAccessFailed, user, "{0}", message);
        }

        public abstract bool IsBlankAsset(in ResAssetCallTable table);
        public abstract Pointer<char> GetBoneName(in ResAssetCallTable table);
        public abstract bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param);
        public abstract Pointer<char> GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param);
        public abstract bool IsAutoOneTimeFade(in ResAssetCallTable table);
        public abstract bool IsForceLoopAsset(in ResAssetCallTable table);
        public abstract float GetDelayWithOverwrite(in ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance);
        public abstract float GetDuration(in ResAssetCallTable table, UserInstance instance);
        protected abstract int GetTriggerOverwriteParamId(uint reff);
        protected abstract uint GetAssetBitFlag(in ResAssetCallTable table);
    }
}

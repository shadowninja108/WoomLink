using System;
using WoomLink.sead;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;

namespace WoomLink.xlink2
{
    public abstract class ResourceAccessor
    {
        public Pointer<ResUserHeader> HeaderPointer;
        public UserResource Resource;
        public System System;

        protected ResourceAccessor(UserResource resource, System system)
        {
            Resource = resource;
            System = system;
        }

        public bool CheckAndErrorIsAsset(ref ResAssetCallTable table, string caller)
        {
            if (!table.IsContainer)
                return true;

            /* TODO: print error. */
            return false;
        }

        protected Pointer<ResParam> GetResParamFromAssetParamPos(Pointer<ResAssetParam> param, uint reff)
        {
            ref var resParam = ref param.Ref;
            if (resParam.IsParamDefault(reff))
                return param.GetValues().Add((UintPointer)(BitFlagUtil.CountRightOnBit64(resParam.Bitfield, (int)reff) - 1));
            else
                return Pointer<ResParam>.Null;
        }

        protected string GetResParamValueString(ref ResParam param)
        {
            var nameTable = Resource.CurrentParam.Common.Value.NameTable;
            return Pointer<char>.As(param.Value + nameTable).AsString();
        }

        protected int GetResParamValueInt(ref ResParam param)
        {
            return Resource.CurrentParam.Common.Value.DirectValueTableSpanAsInts[(int)param.Value];
        }

        protected float GetResParamValueFloat(ref ResParam param, UserInstance instance)
        {
            switch (param.ReferenceType)
            {
                case ValueReferenceType.Direct:
                    return Resource.CurrentParam.Common.Value.DirectValueTableSpanAsFloats[(int)param.Value];

                case ValueReferenceType.Curve:
                    return GetCurveValue(
                        ref Resource.CurrentParam.Common.Value.CurveTableSpan[(int)param.Value],
                        instance
                    );
                case ValueReferenceType.Unk3:
                {
                    if (Resource.CurrentParam.Common.Value.RandomTable.IsNull())
                        return 0;

                    ref var random = ref Resource.CurrentParam.Common.Value.RandomTableSpan[(int)param.Value];
                    var range = random.MaxValue - random.MinValue;

                    return random.MinValue + range * System.Rnd.GetSingle();
                }
                case ValueReferenceType.Unk6:
                {
                    if (Resource.CurrentParam.Common.Value.RandomTable.IsNull())
                        return 0;

                    ref var random = ref Resource.CurrentParam.Common.Value.RandomTableSpan[(int)param.Value];
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
                    /* TODO: setError */
                    throw new NotImplementedException();
            }
        }

        public float GetCurveValue(ref ResCurveCallTable table, UserInstance instance)
        {
            throw new NotImplementedException();
        }

        protected string GetResOverwriteParamValueString(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff)
        {
            var id = GetTriggerOverwriteParamId(reff);

            if (paramPtr.IsNull())
                return "";

            if (id < 0)
                return "";

            ref var param = ref paramPtr.Ref;
            if (param.IsParamDefault(reff))
                return "";

            var index = BitFlagUtil.CountRightOnBit(param.Bitfield, (int)reff);
            return GetResParamValueString(ref paramPtr.GetValuesSpan()[index]);
        }

        protected float GetResOverwriteParamValueFloat(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff, UserInstance instance)
        {
            var id = GetTriggerOverwriteParamId(reff);
            if (paramPtr.IsNull())
                return 0;
            if (id < 0)
                return 0;
            
            ref var param = ref paramPtr.Ref;
            if(param.IsParamDefault(reff)) 
                return 0;

            ref var define = ref paramPtr.GetValuesSpan()[BitFlagUtil.CountRightOnBit(param.Bitfield, (int)reff)];
            return GetResParamValueFloat(ref define, instance);
        }

        public bool IsParamOverwritten(Pointer<ResTriggerOverwriteParam> paramPtr, uint reff)
        {
            var id = GetTriggerOverwriteParamId(reff);

            if (paramPtr.IsNull())
                return false;
            
            if(id < 0)
                return false;

            return paramPtr.Ref.IsParamDefault(reff);
        }


        public abstract bool IsBlankAsset(ref ResAssetCallTable table);
        public abstract string GetBoneName(ref ResAssetCallTable table);
        public abstract bool IsBoneNameOverwritten(Pointer<ResTriggerOverwriteParam> param);
        public abstract string GetOverwriteBoneName(Pointer<ResTriggerOverwriteParam> param);
        public abstract bool IsAutoOneTimeFade(ref ResAssetCallTable table);
        public abstract bool IsForceLoopAsset(ref ResAssetCallTable table);
        public abstract float GetDelayWithOverwrite(ref ResAssetCallTable table, Pointer<ResTriggerOverwriteParam> overwriteParam, UserInstance instance);
        public abstract float GetDuration(ref ResAssetCallTable table, UserInstance instance);
        protected abstract int GetTriggerOverwriteParamId(uint reff);
        protected abstract int GetAssetBitFlag(ref ResAssetCallTable table);
    }
}

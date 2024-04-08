using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WoomLink.Ex;
using WoomLink.sead;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.Properties;
using WoomLink.xlink2.User.Instance;
using WoomLink.xlink2.User.Resource;

#if XLINK_VER_BLITZ || XLINK_VER_PARK
using PropertyIdxType = System.Int16;
#elif XLINK_VER_THUNDER || XLINK_VER_EXKING
using PropertyIdxType = System.Int32;
#else
#error Invalid XLink version target.
#endif

namespace WoomLink.xlink2
{

    public abstract class System
    {
        public ResourceBuffer ResourceBuffer;
        public List<User.User> UserList = new();
        public int Unk28 = 0;
        public uint EventPoolNum;
        public uint Field30;
        public uint CurrentEventId;
        public byte Field38;
        public PropertyDefinition[] GlobalPropertyDefinitions { get; set; }
        public uint[] GlobalPropertyValues { get; set; }
        public ulong GlobalPropertyValueUsedBitfield = 0;
        public byte Field58;
        public ErrorMgr ErrorMgr;
        public bool CallEnable = true;
        //public PtrArray<User> GlobalPropertyTriggerUserList = new();
        public SeadRandom Rnd;

        protected System()
        {
            ErrorMgr = new ErrorMgr(this);;
            Rnd.Init();
        }

        protected void InitSystem_(/* initializing heap */ /* pool for continual use */ uint eventPoolNum)
        {
            ResourceBuffer = new ResourceBuffer();
            EventPoolNum = eventPoolNum;
            Field30 = 0;
            //GlobalPropertyTriggerUserList.AllocBuffer(96);
        }

        public bool LoadResource(UintPointer pointer)
        {
            return ResourceBuffer.Load(pointer, this);
        }

        public void AllocGlobalProperty(uint count /* heap */)
        {
            GlobalPropertyDefinitions = new PropertyDefinition[count];
            GlobalPropertyValues = new uint[count];
            /* TODO: Editor buffer */
            Field58 = 1;
        }

        public void SetGlobalPropertyDefinition(uint index, PropertyDefinition definition)
        {
            if(definition == null)
                return;
            if (index > GlobalPropertyDefinitions.Length)
                return;

            foreach (var d in GlobalPropertyDefinitions)
            {
                if(d == null)
                    continue;
                if (d.Name == definition.Name)
                    return;
            }

            GlobalPropertyDefinitions[index] = definition;
            if ((byte)definition.Type < (byte)PropertyType.End)
                GlobalPropertyValues[index] = 0;
        }

        public void FixGlobalPropertyDefinition()
        {
            ResourceBuffer.ApplyGlobalPropertyDefinition(this);
            /* TODO: Editor buffer */
            Field58 = 1;
        }

        public PropertyIdxType SearchGlobalPropertyIndex(string name)
        {
            for (var i = 0; i < GlobalPropertyDefinitions.Length; i++)
            {
                var def = GlobalPropertyDefinitions[i];
                if (def.Name == name)
                    return (PropertyIdxType)i;
            }

            return -1;
        }

        public void RegisterUserForGlobalPropertyTrigger(User.User user)
        {
            if(!user.UserResource.HasGlobalPropertyTrigger())
                return;

            /*
            /* If there's nothing in the array, just insert and we're done. #1#
            var count = GlobalPropertyTriggerUserList.Count;
            if (count <= 0)
            {
                GlobalPropertyTriggerUserList.Add(user);
                return;
            }

            /* Be sure we do not duplicate the user being in this array. #1#
            for (var i = 0; i < count; i++)
            {
                if (GlobalPropertyTriggerUserList[i] == user)
                    return;
            }

            /* User isn't in the array, add. #1#
            GlobalPropertyTriggerUserList.Add(user);
            */
        }

        protected User.User? SearchUserOrCreate(UserInstance.CreateArg arg /* heap */, uint unk)
        {
            var loc = GetModuleLockObj();
            User.User? foundUser = null;
            loc.Lock();
            foreach (var user in UserList)
            {
                if(user.Unk40 != unk)
                    continue;
                if(user.Name != arg.Name)
                    continue;
                /* Compare by heap. */

                foundUser = user;
                break;
            }
            loc.Unlock();
            
            if (foundUser != null)
            {
                return foundUser;
            }

            /* Check if the user would fit in the heap. */

            foundUser = new User.User(arg.Name, this, unk);

            string[]? actionSlotStrings = null;
            if (arg.ActionSlotNames != null && arg.ActionSlotNames.Length > 0)
            {
                actionSlotStrings = (string[])arg.ActionSlotNames.Clone();
            }
            foundUser.SetActionSlot(actionSlotStrings);
            foundUser.CreatePropertyDefinitionTable(arg.LocalPropertyCount);

            return foundUser;
        }

        public void FreeEvent(Event even, List<Event>? list)
        {
            list?.Remove(even);
            even.Finalize();
        }

        public void AddError(Error.Type type, User.User? user, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object[] args)
        {
            /* Inferred. */
            ErrorMgr.Add(type, user, string.Format(format, args));
        }

        public Pointer<ResUserHeader> GetResUserHeader(string name)
        {
            return ResourceBuffer.SearchResUserHeader(name);
        }

        public ref ParamDefineTable GetParamDefineTable()
        {
            return ref ResourceBuffer.PDT;
        }

        public ref ParamDefineTable GetParamDefineTable(ResMode mode)
        {
            Debug.Assert(mode == ResMode.Normal);
            return ref ResourceBuffer.PDT;
        }

        /* getNodeClassType */
        /* drawInformation */
        /* drawInformation3D */

        public abstract UserResource CreateUserResource(User.User user /* Heap */);

        public abstract uint GetUserParamNum();
        public abstract string GetModuleName();
        public abstract ILockProxy GetModuleLockObj();
        public abstract int GetResourceVersion();

    }
}

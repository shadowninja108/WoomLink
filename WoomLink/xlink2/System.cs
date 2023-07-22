using System.Collections.Generic;
using WoomLink.sead;
using WoomLink.xlink2.File;
using WoomLink.xlink2.File.Enum;
using WoomLink.xlink2.File.Res;
using WoomLink.xlink2.Properties;

#if XLINK_VER_THUNDER
using PropertyIdxType = System.Int32;
#elif XLINK_VER_BLITZ
using PropertyIdxType = System.Int16;
#endif

namespace WoomLink.xlink2
{

    public abstract class System
    {
        public ResourceBuffer ResourceBuffer;
        public List<User> UserList = new();
        public int Unk28 = 0;
        public PropertyDefinition[] GlobalPropertyDefinitions { get; set; }
        public uint[] GlobalPropertyValues { get; set; }
        public bool Field58 = false;
        public bool CallEnable = true;
        public PtrArray<User> GlobalPropertyTriggerUserList = new(96);
        public SeadRandom Rnd;

        protected System()
        {
            Rnd.Init();
        }

        protected void InitSystem_(/* initializing heap */ /* pool for continual use */ uint eventPoolNum)
        {
            ResourceBuffer = new ResourceBuffer();
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
            Field58 = true;
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
            Field58 = true;
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

        public void RegisterUserForGlobalPropertyTrigger(User user)
        {
            if(!user.UserResource.HasGlobalPropertyTrigger())
                return;

            /* If there's nothing in the array, just insert and we're done. */
            var count = GlobalPropertyTriggerUserList.Count;
            if (count <= 0)
            {
                GlobalPropertyTriggerUserList.Add(user);
                return;
            }

            /* Be sure we do not duplicate the user being in this array. */
            for (var i = 0; i < count; i++)
            {
                if (GlobalPropertyTriggerUserList[i] == user)
                    return;
            }

            /* User isn't in the array, add. */
            GlobalPropertyTriggerUserList.Add(user);
        }

        protected User SearchUserOrCreate(UserInstance.CreateArg arg /* heap*/, uint unk)
        {
            var loc = GetModuleLockObj();
            User foundUser = null;
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

            foundUser = new User(arg.Name, this, unk);

            string[] actionSlotStrings = null;
            if (arg.ActionSlotNames != null && arg.ActionSlotNames.Length > 0)
            {
                actionSlotStrings = (string[])arg.ActionSlotNames.Clone();
            }
            foundUser.SetActionSlot(actionSlotStrings);
            foundUser.CreatePropertyDefinitionTable(arg.LocalPropertyCount);

            return foundUser;
        }

        public void FreeEvent(Event even, List<Event> list)
        {
            if (even != null)
                list.Remove(even);
            even.Finalize();
        }

        public Pointer<ResUserHeader> GetResUserHeader(string name)
        {
            return ResourceBuffer.SearchResUserHeader(name);
        }

        public ref ParamDefineTable GetParamDefineTable()
        {
            return ref ResourceBuffer.PDT;
        }

        /* getNodeClassType */
        /* drawInformation */
        /* drawInformation3D */

        public abstract UserResource CreateUserResource(User user /* Heap */);

        public abstract uint GetUserParamNum();
        public abstract string GetModuleName();
        public abstract ILockProxy GetModuleLockObj();
        public abstract int GetResourceVersion();

    }
}

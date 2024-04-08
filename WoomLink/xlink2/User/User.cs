using System;
using System.Collections.Generic;
using WoomLink.xlink2.Properties;
using WoomLink.xlink2.User.Instance;
using WoomLink.xlink2.User.Resource;

namespace WoomLink.xlink2.User
{
    public class User
    {
        public string Name;
        public UserResource UserResource;
        public List<UserInstance> Instances = new();
        /* Heap */
        public uint Unk40;
        /* ushort PropertyDefinitionTableCount */
        /* ushort ActionSlotCount */
        public PropertyDefinition[]? PropertyDefinitionTable = null;
        public string[]? ActionSlots;
        public byte Unk58 = 0;

        public User(string name /* Heap */, System system, uint unk)
        {
            Unk40 = unk;
            Name = name;

            if (system.Unk28 > 0)
            {
               /* This would copy the string, but it's immutable here anyways. */
               Unk58 |= 2;
            }

            UserResource = system.CreateUserResource(this);
        }

        public void SetActionSlot(string[]? actionSlotStrings)
        {
            ActionSlots = actionSlotStrings;
        }

        public void CreatePropertyDefinitionTable(uint count)
        {
            if(count > 0)
                PropertyDefinitionTable = new PropertyDefinition[count];
            else
                PropertyDefinitionTable = Array.Empty<PropertyDefinition>();
        }

        public System GetSystem() => UserResource.GetSystem();

        public UserInstance? GetLeaderInstance()
        {
            if(Instances.Count == 0)
                return null;

            return Instances[0];
        }
    }
}

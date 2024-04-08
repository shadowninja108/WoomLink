using System;

namespace WoomLink.xlink2
{
    public struct Group
    {
        public string Name;
        public int Id;
        public int FieldC;
    }

    public class GroupTable
    {
        public Group[] Groups;

        public GroupTable(int size /* heap */)
        {
            Groups = new Group[size];
        }

        public void BatchEntry(params string[] entries)
        {
            for (var i = 0; i < entries.Length; i++)
            {
                Groups[i].Id = i;
                Groups[i].Name = entries[i];
            }
        }

        public int GetId(string name)
        {
            for (var i = 0; i < Groups.Length; i++)
            {
                if (Groups[i].Name != name)
                    continue;

                return Groups[i].Id;
            }
            return -1;
        }

        public int GetKeyLength(uint idx) => Groups[idx].Name.Length;

        public string SearchKey(int id)
        {
            foreach (ref var group in Groups.AsSpan())
            {
                if(group.Id != id)
                    continue;

                return group.Name;
            }

            return "";
        }
    }
}

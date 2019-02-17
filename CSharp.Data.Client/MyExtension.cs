namespace Data
{
    using Data.Models;
    using Microsoft.OData.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class MyExtension
    {
        private static void CastListToDictionaryTree<Y>(this List<Group> groups, DictionaryTree<Group, Y> rootDictionary, List<Group> rootGroups) where Y : class
        {
            foreach (var rootGroup in rootGroups)
            {
                var newRootDictionary = rootDictionary.New(rootGroup);
                var newRootGroups = groups.Where(g => g.ParentId == rootGroup.Id).ToList();
                CastListToDictionaryTree(groups, newRootDictionary, newRootGroups);
            }
        }

        public static DictionaryTree<Group, Y> ToDictionaryTree<Y>(this DataServiceQuery<Group> groupsDb, Func<Group, Y> getKey, Group rootGroup = null) where Y : class
        {
            var groups = groupsDb.ToList();
            var rootGroups = groups.Where(g => g.ParentId == rootGroup?.Id).ToList();
            var root = new DictionaryTree<Group, Y>(getKey, rootGroup);
            groups.CastListToDictionaryTree(root, rootGroups);
            return root;
        }
    }
}

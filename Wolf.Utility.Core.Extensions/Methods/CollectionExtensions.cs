using System.Collections.Generic;
using System.Linq;

namespace Wolf.Utility.Core.Extensions.Methods
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// removes the element at the specified index and then returns the list worked on.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<T> RemoveAtAndReturn<T>(this List<T> list, int index)
        {
            list.RemoveAt(index);
            return list;
        }

        public static bool EnsureContains<T>(this IEnumerable<T> list, IEnumerable<T> mustContain) 
        {
            var matches = new List<bool>();
            foreach (var must in mustContain) 
            {
                if (list.Contains(must)) matches.Add(true);
                else matches.Add(false);
            }
            return matches.All(x => x == true);
        }
    }
}

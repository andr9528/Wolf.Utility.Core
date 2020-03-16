using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Main.Extensions
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
    }
}

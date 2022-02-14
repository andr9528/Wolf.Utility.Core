using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Extensions.Methods
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The class that the method is contained within</typeparam>
        /// <param name="methodName">Name of the method to retrieve MethodInfo of</param>
        /// <returns>MethodInfo of the Method specifed by the methodName parameter</returns>
        /// <exception cref="NullReferenceException">Thrown when no methods with the specified name is found</exception>
        /// <exception cref="AmbiguousMatchException">Thrown when more than one methods with the specified name is found</exception>
        public static MethodInfo GetMethodInfo<T>(string methodName)
        {
            var type = typeof(T);
            try
            {
                var method = type.GetMethod(methodName);
                return method;
            }
            catch (AmbiguousMatchException ame)
            {
                throw new Exception($"Multiple methods exist with the name: {methodName}", ame);
            }
            catch (NullReferenceException nre)
            {
                throw new Exception($"No method exist with the name: {methodName}", nre);
            }


        }
        // https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
        /// <summary>
        /// By default gets all public constants defines in the type supplied. 
        /// Can be supplied with a different BindingFlags combination to get another subset of Fields.
        /// Substituting the BindingFlags.Public with BindingFlags.NonPublic to get the private or internal fields.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetConstants(this Type type, 
            BindingFlags flags = BindingFlags.Public |
                BindingFlags.Static | BindingFlags.FlattenHierarchy) 
        {
            FieldInfo[] fieldInfos = type.GetFields(flags);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }
    }
}

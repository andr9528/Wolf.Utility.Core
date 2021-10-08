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
    }
}

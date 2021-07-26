using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Core.EntityModules.Core;
using System.Reflection;
using System.Linq;

namespace Wolf.Utility.Core.EntityModules.Modules
{
    public class CopyModule : IModule
    {
        public ModuleType GetModuleType()
        {
            return ModuleType.Copy;
        }
        /// <summary>
        /// Uses Reflection to create and copy properties from the supplied object into a new instace, thereby creating a new reference.
        /// </summary>
        /// <typeparam name="T">The type to copy from and to</typeparam>
        /// <param name="old">The old instance to copy from</param>
        /// <param name="skippedProperties">The properties, by name, to skip when copying. If left empty every property will be copied</param>
        /// <returns></returns>
        public T ToNew<T>(T old, params string[] skippedProperties) where T : class 
        {
            var properties = PropertyInfos<T>();
            var fresh = (T)Activator.CreateInstance(typeof(T));

            // Creates a list of properties, excludeing the skipped ones.
            var propertiesToCopy = properties.Select(x => x).Where(x => !skippedProperties.Contains(x.Name)).ToList();

            foreach (var prop in propertiesToCopy)
            {
                prop.SetValue(fresh, prop.GetValue(old));
            }

            return fresh;
        }

        /// <summary>
        /// Uses Reflection to copy properties from the supplied fresh object into an old instance, thereby reusing a reference.
        /// </summary>
        /// <typeparam name="T">The type to copy from and to</typeparam>
        /// <param name="old">The old instance to copy to</param>
        /// <param name="fresh">The new instance to copy from</param>
        /// <param name="skippedProperties">The properties, by name, to skip when copying. If left empty every property will be copied</param>
        /// <returns></returns>
        public T ToOld<T>(T old, T fresh, params string[] skippedProperties) where T : class
        {
            var properties = PropertyInfos<T>();

            // Creates a list of properties, excludeing the skipped ones.
            var propertiesToCopy = properties.Select(x => x).Where(x => !skippedProperties.Contains(x.Name)).ToList();

            foreach (var prop in propertiesToCopy)
            {
                prop.SetValue(old, prop.GetValue(fresh));
            }

            return old;
        }

        private static PropertyInfo[] PropertyInfos<T>() where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            return properties;
        }
    }
}

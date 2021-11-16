using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wolf.Utility.Core.Deprecated.Factory
{
    public class GenericFactory
    {
        // Assistance with Setter Accessibility: https://stackoverflow.com/questions/3762456/how-to-check-if-property-setter-is-public
        private static T CopyPropertiesToNew<T>(T newObject, T oldObject, bool ignoreDefaults = true,
            bool skipSelectedProperties = true, params string[] skippedProperties) where T : class
        {
            var properties = PropertyInfos<T>();

            foreach (var property in properties)
            {
                if (FlowController(oldObject, ignoreDefaults, skipSelectedProperties, skippedProperties, property)) continue;

                property.SetValue(newObject, property.GetValue(oldObject));
            }

            return newObject;
        }

        private static T CopyPropertiesToOld<T>(T newObject, T oldObject, bool ignoreDefaults = true,
            bool skipSelectedProperties = true, params string[] skippedProperties) where T : class
        {
            var properties = PropertyInfos<T>();

            foreach (var property in properties)
            {
                if (FlowController(oldObject, ignoreDefaults, skipSelectedProperties, skippedProperties, property)) continue;

                property.SetValue(oldObject, property.GetValue(newObject));
            }

            return oldObject;
        }

        private static PropertyInfo[] PropertyInfos<T>() where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            return properties;
        }

        private static bool FlowController<T>(T oldObject, bool ignoreDefaults, bool skipSelectedProperties,
            string[] skippedProperties, PropertyInfo property) where T : class
        {
            if (ignoreDefaults && property.GetValue(oldObject) == default)
                return true;
            if (skipSelectedProperties && skippedProperties.Contains(property.Name))
                return true;
            if (!property.CanWrite)
                return true;
            return false;
        }

        /// <summary>
        /// If the 'toNewObject' is true, then values will be taken from the old one and put into the new one, and then returning the new one.
        /// If it is false, then it does the opposite, taking from the new and putting into the old.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newObject"></param>
        /// <param name="oldObject"></param>
        /// <param name="ignoreDefaults"></param>
        /// <param name="skipSelectedProperties"></param>
        /// <param name="toNewObject"></param>
        /// <param name="skippedProperties"></param>
        /// <returns></returns>
        public static T CopyProperties<T>(T newObject, T oldObject, bool ignoreDefaults = true, bool toNewObject = true,
            bool skipSelectedProperties = true, params string[] skippedProperties) where T : class
        {
            switch (toNewObject)
            {
                case true:
                    return CopyPropertiesToNew(newObject, oldObject, ignoreDefaults, skipSelectedProperties,
                        skippedProperties);
                case false:
                    return CopyPropertiesToOld(newObject, oldObject, ignoreDefaults, skipSelectedProperties,
                        skippedProperties);
            }

            // If this isn't there, then all path does not return an value, even thought it is unreachable
            return null;
        }
    }
}

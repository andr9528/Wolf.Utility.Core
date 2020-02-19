using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wolf.Utility.Main.Factory
{
    public class GenericFactory
    {
        // Assistance with Setter Accessibility: https://stackoverflow.com/questions/3762456/how-to-check-if-property-setter-is-public
        public static T CopyProperties<T>(T newObject, T oldObject, bool ignoreDefaults = true,
            bool skipSelectedProperties = true, params string[] skippedProperties) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (ignoreDefaults && property.GetValue(oldObject) == default)
                    continue;
                if (skipSelectedProperties && skippedProperties.Contains(property.Name))
                    continue;
                if (!property.CanWrite)
                    continue;
                
                property.SetValue(newObject, property.GetValue(oldObject));
            }

            return newObject;
        }
    }
}

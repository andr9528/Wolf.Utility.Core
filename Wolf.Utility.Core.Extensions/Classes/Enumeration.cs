using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wolf.Utility.Core.Extensions.Classes
{
    public class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name)
        {
            (Id, Name) = (id, name);
        }

        public override string ToString()
        {
            return Name;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null)).Cast<T>();
        }

        public static T GetByName<T>(string name) where T : Enumeration
        {
            Type type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(field => (Enumeration) field.GetValue(null))
                .Where(value => value != null && value.Name == name).Cast<T>().FirstOrDefault();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
                return false;

            bool typeMatches = GetType().Equals(obj.GetType());
            bool valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other)
        {
            return Id.CompareTo(((Enumeration) other).Id);
        }
    }
}
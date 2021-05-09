using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheScoreBook.models.enums
{
    public abstract class EnumClass : IComparable
    {
        public string Name { get; }
        public int Id { get; }

        protected EnumClass(string name, int id) => (Name, Id) = (name, id);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : EnumClass =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly).Select(f => f.GetValue(null)).Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not EnumClass otherValue)
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object obj) => Id.CompareTo(((EnumClass) obj).Id);

        public static bool operator ==(EnumClass a, EnumClass b)
            => a is not null && b is not null && a.Id == b.Id && a.Name == b.Name;

        public static bool operator !=(EnumClass a, EnumClass b)
            => !(a == b);
    }
}
using System;
using System.Collections.Generic;
using TheScoreBook.exceptions;
using TheScoreBook.localisation;

namespace TheScoreBook.models.enums.enumclass
{
    public class Style : EnumClass
    {
        public static readonly Style RECURVE = new("Recurve", 0);
        public static readonly Style BAREBOW = new("Barebow", 1);
        public static readonly Style COMPOUND = new("Compound", 2);
        public static readonly Style LONGBOW = new("Longbow", 3);
        public static readonly Style AFB = new("AFB", 4);
        public static readonly Style OTHER = new("Other", 5);

        private Style(string name, int id) : base(name, id) { }

        private static HashSet<int> usedIds;
        protected override void AddUsedIDToSet(int id)
        {
            usedIds ??= new();

            if (!usedIds.Add(id))
                throw new IDAlreadyInUseException($"ID {id} already in use for enum type {GetType()}");
        }

        public override string ToString()
            => LocalisationManager.Instance[Name] ?? Name;

        public static explicit operator Style(int id)
            => id switch
            {
                0 => RECURVE,
                1 => BAREBOW,
                2 => COMPOUND,
                3 => LONGBOW,
                4 => AFB,
                5 => OTHER,
                _ => throw new InvalidCastException($"{id} is not a valid style")
            };
    }
}
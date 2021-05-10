using System;
using TheScoreBook.localisation;

namespace TheScoreBook.models.enums.enumclass
{
    public class Style : EnumClass
    {
        public static Style RECURVE => new("Recurve", 0);
        public static Style BAREBOW => new("Barebow", 1);
        public static Style COMPOUND => new("Compound", 2);
        public static Style LONGBOW => new("Longbow", 3);
        public static Style AFB => new("AFB", 4);
        public static Style OTHER => new("Other", 5);

        public Style(string name, int id) : base(name, id) { }

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
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
    }
}
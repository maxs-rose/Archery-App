namespace TheScoreBook.models.enums
{
    public class Location : EnumClass
    {
        public static readonly Location INDOOR = new(nameof(INDOOR), 0);
        public static readonly Location OUTDOOR = new(nameof(OUTDOOR), 1);
        
        private Location(string name, int id) : base(name, id) {}
    }
}
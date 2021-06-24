using System;
using System.Linq;

namespace TheScoreBook.models.enums
{
    public class RoundGrouping : EnumClass
    {
        public static readonly RoundGrouping FITAIndoor = new RoundGrouping(nameof(FITAIndoor), 0);
        public static readonly RoundGrouping FITAOutdoor = new RoundGrouping(nameof(FITAOutdoor), 1);
        public static readonly RoundGrouping GNASMetricOutdoor = new RoundGrouping(nameof(GNASMetricOutdoor), 2, "GNAS Metric Outdoor");
        public static readonly RoundGrouping GNASImperialOutdoor = new RoundGrouping(nameof(GNASImperialOutdoor), 3, "GNAS Imperial Outdoor");
        public static readonly RoundGrouping GNASIndoor = new RoundGrouping(nameof(GNASIndoor), 4);
        public static readonly RoundGrouping Other = new RoundGrouping(nameof(Other), 5, "Other");
        
        public string DisplayName { get; }
        
        private RoundGrouping(string name, int id) : this(name, id, $"{name[..4]} {name[4..]}" ) { }
        private RoundGrouping(string name, int id, string displayName) : base(name, id)
        {
            DisplayName = displayName;
        }

        public static explicit operator RoundGrouping(int id)
        {
            var res = GetAll<RoundGrouping>().FirstOrDefault(r => r.Id == id);

            if (res == default)
                throw new InvalidCastException($"ID {id} is not a valid id for {nameof(RoundGrouping)}");

            return res;
        }

        public static explicit operator RoundGrouping(string name)
        {
            var res = GetAll<RoundGrouping>().FirstOrDefault(r => r.Name == name);

            if (res == default)
                throw new InvalidCastException($"{nameof(RoundGrouping)} with name {name} does not exist!");

            return res;
        }
    }
}
using System;
using System.Linq;

namespace TheScoreBook.models.enums.enumclass
{
    public class RoundGrouping : EnumClass
    {
        public static RoundGrouping FITAIndoor => new RoundGrouping(nameof(FITAIndoor), 0);
        public static RoundGrouping FITAOutdoor => new RoundGrouping(nameof(FITAOutdoor), 1);
        public static RoundGrouping GNASMetricOutdoor => new RoundGrouping(nameof(GNASMetricOutdoor), 2);
        public static RoundGrouping GNASImperialOutdoor => new RoundGrouping(nameof(GNASImperialOutdoor), 3);
        public static RoundGrouping GNASIndoor => new RoundGrouping(nameof(GNASIndoor), 4);
        public static RoundGrouping Other => new RoundGrouping(nameof(Other), 5);
        
        public RoundGrouping(string name, int id) : base(name, id)
        {
            
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
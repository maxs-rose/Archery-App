using System;
using System.Collections.Generic;
using System.Linq;

namespace TheScoreBook.models.enums
{
    public class MeasurementUnit : EnumClass
    {
        public static readonly MeasurementUnit YD = new (nameof(YD), 0);
        public static readonly MeasurementUnit M = new (nameof(M), 1);
        public static readonly MeasurementUnit CM = new (nameof(CM), 2);
        public static readonly MeasurementUnit IN = new (nameof(IN), 3);
        
        private MeasurementUnit(string name, int id) : base(name, id) { }

        public static explicit operator MeasurementUnit(int id)
        {
            var result = GetAll<MeasurementUnit>().FirstOrDefault(d => d.Id == id);

            if (result == default)
                throw new InvalidCastException($"Could not find ID {id}");

            return result;
        }

        public static explicit operator MeasurementUnit(string unit)
        {
            var result = GetAll<MeasurementUnit>().FirstOrDefault(d => String.Equals(d.Name, unit, StringComparison.InvariantCultureIgnoreCase));

            if (result == default)
                throw new InvalidCastException($"Could not find ID {unit}");

            return result;
        }

        public static IEnumerable<MeasurementUnit> DistanceUnits() => new[] { YD, M };
        public static IEnumerable<MeasurementUnit> TargetUnits() => new[] {CM, IN };

        public override string ToString() => Name.ToLower();
    }
}
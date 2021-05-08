using System;
using System.Collections;
using Xamarin.Forms;

namespace TheScoreBook.models.enums
{
    public class ScoringType : EnumClass
    {
        public static ScoringType TenZone => new("TenZone", 0);
        public static ScoringType FiveZone => new("FiveZone", 1);

        private ScoringType(string name, int id) : base(name, id) { }

        public override string ToString()
            => Id switch
            {
                0 => "X, 10, 9, 8, ..., 1, M",
                1 => "9, 7, 5, 3, 1, M",
                _ => throw new ArgumentOutOfRangeException()
            };

        public EScore MaxScore() => Id == 0 ? EScore.X : EScore.NINE;

        public static explicit operator ScoringType(string obj)
            => obj switch
            {
                "TenZone" => TenZone,
                "FiveZone" => FiveZone,
                _ => throw new InvalidCastException()
            };
    }
}